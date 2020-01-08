module CurrentCipav

open Domain

let ratioUpTo1Pass = 0.0823m
let ratioUpTo5Pass = 0.0187m

let cotisationsCnav annualRevenu =
    let upTo1Pass = min pass annualRevenu
    let upTo5Pass = min (5m * pass) annualRevenu
    ratioUpTo1Pass * upTo1Pass, ratioUpTo5Pass * upTo5Pass

// reference : https://www.lacipav.fr/sites/default/files/2019-05/CIPAV%20-%20Guide%20Pratique%20-%20web_3.pdf
let calculateCnavBasePension career =
    let cotisationsTranche1, cotisationsTranche2 = calculateWholeCareer cotisationsCnav career
    let points = cotisationsTranche1 / (ratioUpTo1Pass * 77.18m) + cotisationsTranche2 / (ratioUpTo5Pass * 8_104.8m)
    let validatedQuarters = (career.RetiringAge - career.StartingAge) * 4m
    let requiredQuarters = requiredQuarters career.BirthYear
    let missingQuarters = requiredQuarters - validatedQuarters |> min 20m
    let pensionRate = 1m - (min 12m missingQuarters) * 0.01m - (max 0m (missingQuarters - 12m)) * 0.0125m
    // TODO : surcote not taken in account here
    let pension = points * 0.5690m * pensionRate / 12m
    { Cotisations = cotisationsTranche1 + cotisationsTranche2; MonthlyAmount = pension }

let cotisationsComplementaire annualRevenu =
    match annualRevenu with
    | _ when annualRevenu < 26580m -> 1353m, 36m
    | _ when annualRevenu < 49280m -> 2705m, 72m
    | _ when annualRevenu < 57850m -> 4058m, 108m
    | _ when annualRevenu < 66400m -> 6763m, 180m
    | _ when annualRevenu < 83060m -> 9468m, 252m
    | _ when annualRevenu < 103180m -> 14878m, 396m
    | _ when annualRevenu < 123300m -> 16231m, 432m
    | _ -> 17583m, 468m

let calculateComplementaire career =
    let cotisations, points = calculateWholeCareer cotisationsComplementaire career
    // No surcote/decote according validated quarters ??
    let pension = points * 2.63m / 12m
    { Cotisations = cotisations; MonthlyAmount = pension }

let calculatePension career =
    let pension = {
        ComposedOf = [ calculateCnavBasePension career; calculateComplementaire career ]
        NetReplacementRate = 0m
    }
    { pension with
        NetReplacementRate =
            // reference : https://www.previssima.fr/question-pratique/quelles-sont-les-cotisations-sociales-sur-les-pensions-de-retraite.html
            pension.TotalMonthlyAmount * 0.909m
                / career.EndMonthSalary }
