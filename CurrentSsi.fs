module CurrentSsi

open Domain
open CurrentRegimeGeneralDeBase

// reference: https://www.secu-independants.fr/cotisations/calcul-des-cotisations/taux-de-cotisations/?L=0
// https://www.secu-independants.fr/retraite/calcul-retraite/retraite-base/calcul-droits/
let cotisationsBase annualRevenue =
    let tranche1 = min annualRevenue pass
    let tranche2 = max (annualRevenue - pass) 0m
    // TODO : minimum contribution
    tranche1 * 0.1775m + tranche2 * 0.006m, 0m

let plafondSecuriteSocialeForRetraiteComplementaire = 37_846m

let cotisationsComplementaire annualRevenue =
    let tranche1 = min annualRevenue plafondSecuriteSocialeForRetraiteComplementaire
    let tranche2 = min (max (annualRevenue - pass) 0m) (4m * pass)
    tranche1 * 0.07m + tranche2 * 0.08m, 0m

// reference : https://www.secu-independants.fr/baremes/prestations-vieillesse-et-invalidite-deces/
let calculateCurrentComplementaire career =
    let cotisations, _ = calculateWholeCareer cotisationsComplementaire career
    let points = cotisations / 17.515m
    let validatedQuarters = (career.RetiringAge - career.StartingAge) * 4m
    let requiredQuarters = requiredQuarters career.BirthYear
    let missingQuarters = requiredQuarters - validatedQuarters |> min 20m
    let pensionRate = 1m - (min 12m missingQuarters) * 0.01m - (max 0m (missingQuarters - 12m)) * 0.0125m
    // TODO : surcote not taken in account here
    let pension = points * 0.09925m * pensionRate // monthly point value
    { Cotisations = cotisations; MonthlyAmount = pension }

let calculatePension career =
    let pension = {
        ComposedOf = [ calculateCurrentBasePension cotisationsBase career; calculateCurrentComplementaire career ]
        NetReplacementRate = 0m
    }
    { pension with
        NetReplacementRate =
            // reference : https://www.previssima.fr/question-pratique/quelles-sont-les-cotisations-sociales-sur-les-pensions-de-retraite.html
            pension.TotalMonthlyAmount * 0.909m
                / career.EndMonthSalary }
