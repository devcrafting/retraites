module Domain

// reference : https://www.previssima.fr/question-pratique/quelles-sont-les-cotisations-sociales-sur-les-pensions-de-retraite.html
// TODO : cotisation rate of retired people : 9.1% or 10.1% ?
let netRatioOnPensionBrute = 0.909m

type Career = {
    BirthYear: Year
    StartingAge: decimal
    RetiringAge: decimal
    NotValidatedQuarters: int
    InitialMonthSalary: decimal
    EndMonthSalary: decimal
}
and Year = decimal
type Pension =
    {
        ComposedOf: SubPension list
        NetReplacementRate: decimal
    } with
    member this.TotalCotisations = this.ComposedOf |> List.sumBy (fun x -> x.Cotisations)
    member this.TotalMonthlyAmount = this.ComposedOf |> List.sumBy (fun x -> x.MonthlyAmount)
    member this.TotalMonthlyNetAmount = this.ComposedOf |> List.sumBy (fun x -> x.MonthlyNetAmount)
    member this.GlobalRateOfReturn = this.TotalMonthlyAmount * 12m / this.TotalCotisations
and SubPension =
    {
        Cotisations: decimal
        MonthlyAmount: decimal
    } with
    member this.MonthlyNetAmount = this.MonthlyAmount * netRatioOnPensionBrute
    member this.RateOfReturn = this.MonthlyAmount * 12m / this.Cotisations
    static member (+) (a: SubPension, b: SubPension) =
        { Cotisations = a.Cotisations + b.Cotisations; MonthlyAmount = a.MonthlyAmount + b.MonthlyAmount }
    static member Zero = { Cotisations = 0m; MonthlyAmount = 0m }

let pass = 41_136m
let smic = 1_539.42m
let smicMensuelNet = 1_185.35m
let smicHoraireBrut = 10.15m

// reference : https://www.efl.fr/chiffres-taux/social/salaire/taux_cot.html
let calculateNetSalary grossSalary =
    let annualSalary = grossSalary * 12m
    let tranche1 = min annualSalary pass
    let tranche2 = max 0m (min annualSalary (8m * pass) - pass)
    let cotisations =
        annualSalary * (0.004m + 0.9825m * (0.068m + 0.024m + 0.005m))
        + tranche1 * (0.069m + 0.0315m + 0.0086m + 0.0014m)
        + tranche2 * (0.0864m + 0.0108m + 0.0014m)
    (annualSalary - cotisations) / 12m

let annualSalary career year =
    12m * (career.InitialMonthSalary + (year - 1 |> decimal) * (career.EndMonthSalary - career.InitialMonthSalary) / (career.RetiringAge - career.StartingAge - 1m))

let calculateOneYearOf cotisationsFun career year =
    annualSalary career year
    |> cotisationsFun

let calculateWholeCareer (cotisationsFun: decimal -> decimal * decimal) career =
    [1..(career.RetiringAge - career.StartingAge |> int)]
    |> List.map (calculateOneYearOf cotisationsFun career)
    |> List.unzip
    |> fun (x, y) -> x |> List.sum, y |> List.sum

let requiredQuarters birthDate =
    let oneQuarterEvery3Years =
        if birthDate < 1953m then failwith "not supported"
        if birthDate >= 1973m then 0m else
        (1972 - int birthDate) / 3 + 1 |> decimal
    172m - oneQuarterEvery3Years
