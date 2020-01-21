module CurrentRegimeGeneralDeBase

open Domain

// reference : https://www.service-public.fr/particuliers/vosdroits/F21552
let calculateCurrentBasePension cotisationsFun career =
    let firstBest25YearsSalary = career.InitialMonthSalary + (career.RetiringAge - career.StartingAge - 25m) * (career.EndMonthSalary - career.InitialMonthSalary) / (career.RetiringAge - career.StartingAge)
    let averageBest25YearsSalary = (firstBest25YearsSalary + career.EndMonthSalary) / 2m
    let validatedQuarters = (career.RetiringAge - career.StartingAge) * 4m
    let requiredQuarters = requiredQuarters career.BirthYear
    let missingQuarters = requiredQuarters - validatedQuarters |> min 20m
    // TODO : surcote https://www.la-retraite-en-clair.fr/depart-retraite-age-montant/calculer-retraite/faut-savoir-surcote
    let pensionRate = 0.5m - 0.00625m * missingQuarters
    let pension = (min (averageBest25YearsSalary) (pass / 12m)) * pensionRate * validatedQuarters / requiredQuarters
    let cotisations, _ = calculateWholeCareer cotisationsFun career
    { Cotisations = cotisations; MonthlyAmount = pension }
