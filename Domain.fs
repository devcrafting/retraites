module Domain

type Career = {
    BirthYear: Year
    StartYear: Year // Junuary 1st
    EndYear: Year // Junuary 1st
    NotValidatedQuarters: int
    InitialMonthWage: decimal
    EndMonthWage: decimal
}
and Year = decimal
type Pension =
    {
        ComposedOf: SubPension list
        ReplacementRate: decimal
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
