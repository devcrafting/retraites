#load "Domain.fs"
#load "CurrentSalaries.fs"
#load "ReformedSalaries.fs"

open Domain

let casTypeD = { BirthYear = 1961m; StartingAge = 22m; RetiringAge = 64m; NotValidatedQuarters = 0; InitialMonthWage = 2_895m; EndMonthWage = 2_895m }
let pension = CurrentSalaries.calculatePension casTypeD

let simulationBasedOn computePension linearSalaryIncrease bornIn =
    let startingAge = 22m
    let retiringAge = 64m
    [500m..500m..25m * smic] @ [pass / 12m; 3m * pass / 12m; 8m * pass / 12m]
    |> List.sortBy id
    |> List.map (fun wage -> wage, computePension { BirthYear = bornIn; StartingAge = startingAge; RetiringAge = retiringAge; NotValidatedQuarters = 0; InitialMonthWage = wage; EndMonthWage = wage + linearSalaryIncrease * (retiringAge - startingAge) })

let simulationCurrentBasedOnSameWage = simulationBasedOn CurrentSalaries.calculatePension 0m

let simulationCurrentBasedOnSameWageFixedBirthDate = simulationCurrentBasedOnSameWage 1980m

let simulationReformedBasedOnSameWage = simulationBasedOn ReformedSalaries.calculatePension 0m

let simulationReformedBasedOnSameWageFixedBirthDate = simulationReformedBasedOnSameWage 1980m

#load ".paket/load/net472/XPlot.GoogleCharts.fsx" // to evaluate in FSI .NET 4.7.2
//#load "../../.paket/load/netcoreapp2.2/XPlot.GoogleCharts.fsx" // not working in FSI, but necessary for autocomplete in IDE
open XPlot.GoogleCharts

let rateOfReturnSeries = [
    "Taux de rendement régime de base", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].RateOfReturn)
    "Taux de rendement régime AGIRC/ARRCO", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].RateOfReturn)
    "Taux de rendement régime général actuel", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
    "Taux de rendement régime général réformé", simulationReformedBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
]

let cotisationsSeries = [
    "Cotisations régime de base", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].Cotisations)
    "Cotisations régime AGIRC/ARRCO", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].Cotisations)
    "Cotisations régime général actuel", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
    "Cotisations régime général réformé", simulationReformedBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
]

let monthlyPensionSeries = [
    "Retraite régime de base", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].MonthlyAmount)
    "Retraite régime AGIRC/ARRCO", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].MonthlyAmount)
    "Retraite régime général actuel", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
]

let replacementRateSeries = [
    "Taux de remplacement régime général actuel, né en 1961", simulationCurrentBasedOnSameWage 1961m |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    "Taux de remplacement régime général actuel, né en 1980", simulationCurrentBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    "Taux de remplacement régime général réformé, né en 1980", simulationReformedBasedOnSameWageFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
]

let labels, series = cotisationsSeries |> List.unzip

series
|> Chart.Combo
|> Chart.WithTitle ""
|> Chart.WithXTitle "Salaire mensuel brut"
|> Chart.WithLabels labels
|> Chart.WithOptions(
    Options(
        vAxis = Axis(format = "percent"),
        aggregationTarget = "category",
        selectionMode = "multiple",
        tooltip = Tooltip(trigger = "selection")))
|> Chart.WithLegend true
|> Chart.WithHeight 600
|> Chart.Show
