#load "Domain.fs"
#load "CurrentSalaries.fs"

open Domain

let simulationBasedOnSameWage =
    [500m..500m..25m * smic]
    |> List.map (fun wage -> wage, CurrentSalaries.calculatePension { BirthYear = 1980m; StartYear = 2002m; EndYear = 2045m; NotValidatedQuarters = 0; InitialMonthWage = wage; EndMonthWage = wage;  })

#load ".paket/load/net472/XPlot.GoogleCharts.fsx" // to evaluate in FSI .NET 4.7.2
//#load "../../.paket/load/netcoreapp2.2/XPlot.GoogleCharts.fsx" // not working in FSI, but necessary for autocomplete in IDE
open XPlot.GoogleCharts

let rateOfReturnSeries = [
    "Taux de rendement régime de base", simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].RateOfReturn * 100m)
    "Taux de rendement régime AGIRC/ARRCO", simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].RateOfReturn * 100m)
    "Taux de rendement régime général actuel", simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn * 100m)
]

let cotisationsSeries = [
    simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].Cotisations)
    simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].Cotisations)
    simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
]

let monthlyPensionSeries = [
    simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].MonthlyAmount)
    simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].MonthlyAmount)
    simulationBasedOnSameWage |> Seq.map (fun (x, y) -> x, y.TotalMonthlyAmount)
]

let labels, series = rateOfReturnSeries |> List.unzip

series
|> Chart.Combo
|> Chart.WithTitle ""
|> Chart.WithXTitle "Salaire mensuel brut"
|> Chart.WithLabels labels
|> Chart.Show
