module Domain

type Career = {
    BirthYear: Year
    StartingAge: decimal
    RetiringAge: decimal
    NotValidatedQuarters: int
    InitialMonthWage: decimal
    EndMonthWage: decimal
}
and Year = decimal
type Pension =
    {
        ComposedOf: SubPension list
        NetReplacementRate: decimal
    } with
    member this.TotalCotisations = this.ComposedOf |> List.sumBy (fun x -> x.Cotisations)
    member this.TotalMonthlyAmount = this.ComposedOf |> List.sumBy (fun x -> x.MonthlyAmount)
    member this.GlobalRateOfReturn = this.TotalMonthlyAmount * 12m / this.TotalCotisations
and SubPension =
    {
        Cotisations: decimal
        MonthlyAmount: decimal
    } with
    member this.RateOfReturn = this.MonthlyAmount * 12m / this.Cotisations
    static member (+) (a: SubPension, b: SubPension) =
        { Cotisations = a.Cotisations + b.Cotisations; MonthlyAmount = a.MonthlyAmount + b.MonthlyAmount }
    static member Zero = { Cotisations = 0m; MonthlyAmount = 0m }

let pass = 40_000m
let smic = 1_521.22m

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

let calculateOneYearOf cotisationsFun career year =
    let annualWage = 12m * (career.InitialMonthWage + (year - 1 |> decimal) * (career.EndMonthWage - career.InitialMonthWage) / (career.RetiringAge - career.StartingAge - 1m))
    cotisationsFun annualWage

let calculateWholeCareer (cotisationsFun: decimal -> decimal * decimal) career =
    [1..(career.RetiringAge - career.StartingAge |> int)]
    |> List.map (calculateOneYearOf cotisationsFun career)
    |> List.unzip
    |> fun (x, y) -> x |> List.sum, y |> List.sum
