type Career = {
    BirthYear: Year
    StartYear: Year // Junuary 1st
    EndYear: Year // Junuary 1st
    NotValidatedQuarters: int
    InitialMonthWage: decimal
    EndMonthWage: decimal
}
and Year = decimal

let pass = 40_000m

let requiredQuarters birthDate = 172m // TODO : less for before than 1973

let calculateCurrentBasePension career =
    let firstBest25YearsWage = career.InitialMonthWage + (career.EndYear - career.StartYear - 25m) * (career.EndMonthWage - career.InitialMonthWage) / (career.EndYear - career.StartYear)
    let averageBest25YearsWage = (firstBest25YearsWage + career.EndMonthWage) / 2m
    let validatedQuarters = (career.EndYear - career.StartYear) * 4m
    let requiredQuarters = requiredQuarters career.BirthYear
    let missingQuarters = requiredQuarters - validatedQuarters |> min 20m
    let pensionRate = 0.5m - 0.00625m * missingQuarters
    averageBest25YearsWage * pensionRate * validatedQuarters / requiredQuarters

let calculateAgricArrcoPensionForYear career year =
    let wageForYear = 12m * (career.InitialMonthWage + (year - 1 |> decimal) * (career.EndMonthWage - career.InitialMonthWage) / (career.EndYear - career.StartYear - 1m))
    let firstTrancheWage = min wageForYear pass
    let secondTrancheWage = max 0m (min wageForYear (8m * pass) - pass)
    let cotisations = firstTrancheWage * 0.0787m + secondTrancheWage * 0.2159m
    let points = cotisations / 1.27m / 17.3982m
    let validatedQuarters = (career.EndYear - career.StartYear) * 4m
    let requiredQuarters = requiredQuarters career.BirthYear
    let missingQuarters = requiredQuarters - validatedQuarters |> min 20m
    let pensionRate = 1m - (min 12m missingQuarters) * 0.01m - (max 0m (missingQuarters - 12m)) * 0.0125m
    points * 1.2714m * pensionRate / 12m

let calculateCurrentAgircArrcoPension career =
    [1..(career.EndYear - career.StartYear |> int)]
    |> Seq.sumBy (calculateAgricArrcoPensionForYear career)

let calculateCurrentPension career =
    calculateCurrentBasePension career
    + calculateCurrentAgircArrcoPension career

let calculateExpectedPension career = 0m

let career = { BirthYear = 1981m; StartYear = 2003m; EndYear = 2046m; NotValidatedQuarters = 0; InitialMonthWage = 1394m; EndMonthWage = 2895m }

calculateCurrentPension career