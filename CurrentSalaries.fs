module CurrentSalaries

open Domain

let requiredQuarters birthDate =
    let oneQuarterEvery3Years =
        if birthDate < 1953m then failwith "not supported"
        if birthDate >= 1973m then 0m else
        (1972 - int birthDate) / 3 + 1 |> decimal
    172m - oneQuarterEvery3Years

let cotisationsBase annualWage =
    (min annualWage pass) * 0.1545m + annualWage * 0.023m, 0m

// reference : https://www.service-public.fr/particuliers/vosdroits/F21552
let calculateCurrentBasePension career =
    let firstBest25YearsWage = career.InitialMonthWage + (career.RetiringAge - career.StartingAge - 25m) * (career.EndMonthWage - career.InitialMonthWage) / (career.RetiringAge - career.StartingAge)
    let averageBest25YearsWage = (firstBest25YearsWage + career.EndMonthWage) / 2m
    let validatedQuarters = (career.RetiringAge - career.StartingAge) * 4m
    let requiredQuarters = requiredQuarters career.BirthYear
    let missingQuarters = requiredQuarters - validatedQuarters |> min 20m
    let pensionRate = 0.5m - 0.00625m * missingQuarters
    let pension = (min (averageBest25YearsWage) (pass / 12m)) * pensionRate * validatedQuarters / requiredQuarters
    let cotisations, _ = calculateWholeCareer cotisationsBase career
    { Cotisations = cotisations; MonthlyAmount = pension }

let cotisationsAgircArrco annualWage =
    let firstTrancheWage = min annualWage pass
    let secondTrancheWage = max 0m (min annualWage (8m * pass) - pass)
    firstTrancheWage * 0.0787m + secondTrancheWage * 0.2159m,
        firstTrancheWage * 0.0215m + secondTrancheWage * 0.027m + (if annualWage > pass then firstTrancheWage + secondTrancheWage else 0m) * 0.0035m

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
        ComposedOf = [ calculateCurrentBasePension career; calculateCurrentAgircArrcoPension career ]
        NetReplacementRate = 0m
    }
    { pension with
        NetReplacementRate =
            // reference : https://www.previssima.fr/question-pratique/quelles-sont-les-cotisations-sociales-sur-les-pensions-de-retraite.html
            (pension.ComposedOf.[0].MonthlyAmount * 0.909m + pension.ComposedOf.[1].MonthlyAmount * 0.899m)
                / (calculateNetSalary career.EndMonthWage) }
