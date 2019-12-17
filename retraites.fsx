#load "Domain.fs"
#load "CurrentSalaries.fs"
#load "Reformed.fs"

open Domain

let casTypeD = { BirthYear = 1961m; StartingAge = 22m; RetiringAge = 64m; NotValidatedQuarters = 0; InitialMonthSalary = 2_895m; EndMonthSalary = 2_895m }
let pension = CurrentSalaries.calculatePension casTypeD

let simulationBasedOn computePension linearSalaryIncrease bornIn =
    let startingAge = 22m
    let retiringAge = 64m
    [500m..500m..25m * smic] @ [pass / 12m; 3m * pass / 12m; 8m * pass / 12m]
    |> List.sortBy id
    |> List.map (fun salary -> salary, computePension { BirthYear = bornIn; StartingAge = startingAge; RetiringAge = retiringAge; NotValidatedQuarters = 0; InitialMonthSalary = salary; EndMonthSalary = salary + linearSalaryIncrease * (retiringAge - startingAge) })

let simulationCurrentBasedOnSameSalary = simulationBasedOn CurrentSalaries.calculatePension 0m

let simulationCurrentBasedOnSameSalaryFixedBirthDate = simulationCurrentBasedOnSameSalary 1980m

let simulationReformedBasedOnSameSalary = simulationBasedOn Reformed.calculatePensionSalaries 0m

let simulationReformedBasedOnSameSalaryFixedBirthDate = simulationReformedBasedOnSameSalary 1980m

#load ".paket/load/net472/XPlot.GoogleCharts.fsx" // to evaluate in FSI .NET 4.7.2
//#load "../../.paket/load/netcoreapp2.2/XPlot.GoogleCharts.fsx" // not working in FSI, but necessary for autocomplete in IDE
open XPlot.GoogleCharts

let rateOfReturnSeries = [
    "Taux de rendement régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].RateOfReturn)
    "Taux de rendement régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].RateOfReturn)
    "Taux de rendement régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
    "Taux de rendement régime général réformé", simulationReformedBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
]

let cotisationsSeries = [
    "Cotisations régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].Cotisations)
    "Cotisations régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].Cotisations)
    "Cotisations régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
    "Cotisations régime général réformé", simulationReformedBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
]

let monthlyPensionSeries = [
    "Retraite régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].MonthlyAmount)
    "Retraite régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].MonthlyAmount)
    "Retraite régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
]

let replacementRateSeries = [
    "Taux de remplacement régime général actuel, né en 1961", simulationCurrentBasedOnSameSalary 1961m |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    "Taux de remplacement régime général actuel, né en 1980", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    "Taux de remplacement régime général réformé, né en 1980", simulationReformedBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
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
