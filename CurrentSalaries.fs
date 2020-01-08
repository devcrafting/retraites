module CurrentSalaries

open Domain
open CurrentRegimeGeneralDeBase

let cotisationsBase annualSalary =
    (min annualSalary pass) * 0.1545m + annualSalary * 0.023m, 0m

let cotisationsAgircArrco annualSalary =
    let firstTrancheSalary = min annualSalary pass
    let secondTrancheSalary = max 0m (min annualSalary (8m * pass) - pass)
    // TODO : minimum contribution
    firstTrancheSalary * 0.0787m + secondTrancheSalary * 0.2159m,
        firstTrancheSalary * 0.0215m + secondTrancheSalary * 0.027m + (if annualSalary > pass then firstTrancheSalary + secondTrancheSalary else 0m) * 0.0035m

// reference : https://www.agirc-arrco.fr/particuliers/prevoir-retraite/age-retraite-calcul/
let calculateCurrentAgircArrcoPension career =
    let cotisations, cotisationsWithoutPoints = calculateWholeCareer cotisationsAgircArrco career
    let points = cotisations / 1.27m / 17.3982m
    let validatedQuarters = (career.RetiringAge - career.StartingAge) * 4m
    let requiredQuarters = requiredQuarters career.BirthYear
    let missingQuarters = requiredQuarters - validatedQuarters |> min 20m
    let pensionRate = 1m - (min 12m missingQuarters) * 0.01m - (max 0m (missingQuarters - 12m)) * 0.0125m
    // TODO : the best between this rate and rate depending on age (cf. https://www.agirc-arrco.fr/fileadmin/agircarrco/documents/Doc_specif_page/Coefficients_danticipation_carrieres_courtes.pdf)
    // TODO : surcote not taken in account here
    let pension = points * 1.2714m * pensionRate / 12m
    { Cotisations = cotisations + cotisationsWithoutPoints; MonthlyAmount = pension }

let calculatePension career =
    let pension = {
        ComposedOf = [ calculateCurrentBasePension cotisationsBase career; calculateCurrentAgircArrcoPension career ]
        NetReplacementRate = 0m
    }
    { pension with
        NetReplacementRate =
            // reference : https://www.previssima.fr/question-pratique/quelles-sont-les-cotisations-sociales-sur-les-pensions-de-retraite.html
            (pension.ComposedOf.[0].MonthlyAmount * 0.909m + pension.ComposedOf.[1].MonthlyAmount * 0.899m)
                / (calculateNetSalary career.EndMonthSalary) }
