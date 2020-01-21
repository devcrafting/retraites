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


let private validatedQuartersForYear career year =
    // simplified model
    let quartersNumber = (annualSalary career year) / (50m * smicHoraireBrut)
    quartersNumber
    |> System.Math.Floor
    |> min 12m // no more than 12 months validated

let private ratioFullCareer career =
    let validatedQuarters =
        [1..(career.RetiringAge - career.StartingAge |> int)]
        |> List.sumBy (validatedQuartersForYear career)
    validatedQuarters / (3m * requiredQuarters career.BirthYear)

let calculateMinimalPension career =
    let ratio = ratioFullCareer career
    printfn "%f" ratio
    smicMensuelNet * 0.85m / netRatioOnPensionBrute * ratio

let calculateReformedPension cotisationsFun career =
    let minimalPension = calculateMinimalPension career
    let cotisations, cotisationsWithoutPoints = calculateWholeCareer cotisationsFun career
    let points = cotisations / 10m
    let pension = points * 0.55m * pensionRate career / 12m
    printfn "%f - %f" pension minimalPension
    { Cotisations = cotisations + cotisationsWithoutPoints; MonthlyAmount = max pension minimalPension }

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
            pension.TotalMonthlyNetAmount
                / (calculateNetSalary career.EndMonthSalary) }

let cotisationsNonSalariesReform annualRevenue =
    let upTo1Pass = min (1m * pass) annualRevenue
    let from1PassTo3Pass = min (3m * pass) (max (annualRevenue - pass) 0m)
    upTo1Pass * 0.2531m + from1PassTo3Pass * 0.1013m, annualRevenue * 0.0281m

let calculatePensionNonSalaries career =
    let pension = {
        ComposedOf = [ calculateReformedPension cotisationsNonSalariesReform career ]
        NetReplacementRate = 0m
    }
    { pension with
        NetReplacementRate = // NB: salary for TNS is already net
            pension.TotalMonthlyNetAmount / career.EndMonthSalary }
