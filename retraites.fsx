#load "Domain.fs"
#load "CurrentRegimeGeneralDeBase.fs"
#load "CurrentSalaries.fs"
#load "CurrentSsi.fs"
#load "CurrentCipav.fs"
#load "Reformed.fs"

open Domain

let casTypeD = { BirthYear = 1961m; StartingAge = 22m; RetiringAge = 64m; NotValidatedQuarters = 0; InitialMonthSalary = 2_895m; EndMonthSalary = 2_895m }
let pension = CurrentSalaries.calculatePension casTypeD

let simulationBasedOn computePension linearSalaryIncrease bornIn =
    let startingAge = 22m
    let retiringAge = 64m
    [500m..500m..25m * smic] @ [pass / 12m; 3m * pass / 12m; 4m * pass / 12m; 8m * pass / 12m]
    |> List.sortBy id
    |> List.map (fun salary -> salary, computePension { BirthYear = bornIn; StartingAge = startingAge; RetiringAge = retiringAge; NotValidatedQuarters = 0; InitialMonthSalary = salary; EndMonthSalary = salary + linearSalaryIncrease * (retiringAge - startingAge) })

let simulationCurrentBasedOnSameSalary = simulationBasedOn CurrentSalaries.calculatePension 0m

let simulationCurrentBasedOnSameSalaryFixedBirthDate = simulationCurrentBasedOnSameSalary 1980m

let simulationCurrentSsiBasedOnSameSalaryFixedBirthDate = simulationBasedOn CurrentSsi.calculatePension 0m 1980m

let simulationCurrentCipavBasedOnSameSalaryFixedBirthDate = simulationBasedOn CurrentCipav.calculatePension 0m 1980m

let simulationReformedSalariesBasedOnSameSalary = simulationBasedOn Reformed.calculatePensionSalaries 0m

let simulationReformedSalariesBasedOnSameSalaryFixedBirthDate = simulationReformedSalariesBasedOnSameSalary 1980m

let simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate = simulationBasedOn Reformed.calculatePensionNonSalaries 0m 1980m

#load ".paket/load/net472/XPlot.GoogleCharts.fsx" // to evaluate in FSI .NET 4.7.2
//#load "../../.paket/load/netcoreapp2.2/XPlot.GoogleCharts.fsx" // not working in FSI, but necessary for autocomplete in IDE
open XPlot.GoogleCharts

let rateOfReturnSeries = "Taux de rendement (retraite annuelle/cotisations payées) selon les revenus (fixes) et par régime", [
    // "Régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].RateOfReturn)
    // "Régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].RateOfReturn)
    // "Régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
    // "Régime réformé des salariés", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
    "Régime réformé des non salariés", simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
    "Régime SSI actuel", simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
    "Régime CIPAV actuel", simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
]

let cotisationsSeries = "Cotisations selon les revenus sur toute une carrière (revenus fixes) et par régime", [
    // "Régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].Cotisations)
    // "Régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].Cotisations)
    // "Régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
    // "Régime réformé des salariés", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
    "Régime réformé des non salariés", simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
    "Régime SSI actuel", simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
    "Régime CIPAV actuel", simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
]

let monthlyPensionSeries = "Retraite mensuelle selon les revenus sur toute une carrière (revenus fixes) et par régime", [
    // "Régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].MonthlyAmount)
    // "Régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].MonthlyAmount)
    // "Régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
    // "Régime réformé des salariés", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
    "Régime réformé des non salariés", simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
    "Régime SSI actuel", simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
    "Régime CIPAV actuel", simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
]

let replacementRateSeries = "Taux de remplacement selon les revenus sur toute une carrière (revenus fixes) et par régime", [
    // "Taux de remplacement régime général actuel, né en 1961", simulationCurrentBasedOnSameSalary 1961m |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    // "Régime général actuel, né en 1980", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    // "Régime général réformé, né en 1980", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    "Régime réformé des non salariés", simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    "Régime SSI actuel", simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    "Régime CIPAV actuel", simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
]

let title, (labels, series) = replacementRateSeries |> fun (fst, snd) -> fst, snd |> List.unzip

series
|> Chart.Combo
|> Chart.WithOptions(
    Options(
        vAxis = Axis(format = "percent"),
        // vAxis = Axis(format = "#,###€"),
        hAxis = Axis(viewWindow = ViewWindow(max = 25000)),
        aggregationTarget = "category",
        selectionMode = "multiple",
        tooltip = Tooltip(trigger = "selection")))
|> Chart.WithTitle title
|> Chart.WithLabels labels
//|> Chart.WithXTitle "Salaire mensuel brut"
|> Chart.WithXTitle "Revenu (DSI) mensualisé"
|> Chart.WithLegend true
|> Chart.WithHeight 600
|> Chart.Show
