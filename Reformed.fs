module Reformed

open Domain

let numberOfYearsRequired = 43m
let legalRetirementAge = 62m
let defaultFullPensionRateAge = 64m
let increaseDecreaseRatePerYear = 0.05m

let isLong career =
    min (career.RetiringAge - career.StartingAge - numberOfYearsRequired) 0m = 0m
        && career.StartingAge < defaultFullPensionRateAge - numberOfYearsRequired

let pensionRate career =
    let fullPensionRateAge =
        if isLong career then
            career.RetiringAge + 4m
        else if career.RetiringAge < legalRetirementAge then
            failwith "not supported: not sure it is possible ?"
        else max legalRetirementAge career.RetiringAge
    1m + (fullPensionRateAge - defaultFullPensionRateAge) * increaseDecreaseRatePerYear

let calculateReformedPension cotisationsFun career =
    let cotisations, cotisationsWithoutPoints = calculateWholeCareer cotisationsFun career
    let points = cotisations / 10m
    // TODO : take in account minimum pension
    let pension = points * 0.55m * pensionRate career / 12m
    // TODO : cotisation rate of retired people : 9.1% or 10.1% ?
    { Cotisations = cotisations + cotisationsWithoutPoints; MonthlyAmount = pension * 0.909m }

let cotisationsSalariesReform annualSalary =
    let upTo3Pass = min (3m * pass) annualSalary
    upTo3Pass * 0.2531m, annualSalary * 0.0281m

let calculatePensionSalaries career =
    let pension = {
        ComposedOf = [ calculateReformedPension cotisationsSalariesReform career ]
        NetReplacementRate = 0m
    }
    { pension with
        NetReplacementRate =
            pension.TotalMonthlyAmount
                / (calculateNetSalary career.EndMonthSalary) }
