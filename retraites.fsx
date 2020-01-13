#load "Domain.fs"
#load "CurrentRegimeGeneralDeBase.fs"
#load "CurrentSalaries.fs"
#load "CurrentSsi.fs"
#load "CurrentCipav.fs"
#load "Reformed.fs"

open Domain

let casTypeD = { BirthYear = 1961m; StartingAge = 22m; RetiringAge = 64m; NotValidatedQuarters = 0; InitialMonthSalary = 2_895m; EndMonthSalary = 2_895m }
let pension = CurrentSalaries.calculatePension casTypeD

let simulationBasedOn computePension percentSalaryIncreaseBetweenStartAndRetirement bornIn =
    let startingAge = 22m
    let retiringAge = 64m
    [500m..500m..25m * smic] @ [pass / 12m; 3m * pass / 12m; 4m * pass / 12m; 8m * pass / 12m]
    |> List.sortBy id
    |> List.map (fun salary -> salary, computePension { BirthYear = bornIn; StartingAge = startingAge; RetiringAge = retiringAge; NotValidatedQuarters = 0; InitialMonthSalary = salary; EndMonthSalary = salary * (1m + percentSalaryIncreaseBetweenStartAndRetirement / 100m) })

let simulationCurrentBasedOnSameSalary = simulationBasedOn CurrentSalaries.calculatePension 0m
let simulationCurrentBasedOnSameSalaryFixedBirthDate = simulationCurrentBasedOnSameSalary 1980m

let simulationCurrentSsiBasedOnSameSalaryFixedBirthDate = simulationBasedOn CurrentSsi.calculatePension 0m 1980m
let simulationCurrentSsiBasedOnPercentSalaryIncreaseBirthDate percentIncrease = simulationBasedOn CurrentSsi.calculatePension percentIncrease 1980m

let simulationCurrentCipavBasedOnSameSalaryFixedBirthDate = simulationBasedOn CurrentCipav.calculatePension 0m 1980m
let simulationCurrentCipavBasedOnPercentSalaryIncreaseBirthDate percentIncrease = simulationBasedOn CurrentCipav.calculatePension percentIncrease 1980m

let simulationReformedSalariesBasedOnSameSalary = simulationBasedOn Reformed.calculatePensionSalaries 0m
let simulationReformedSalariesBasedOnSameSalaryFixedBirthDate = simulationReformedSalariesBasedOnSameSalary 1980m

let simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate = simulationBasedOn Reformed.calculatePensionNonSalaries 0m 1980m
let simulationReformedNonSalariesBasedOnPercentSalaryIncreaseFixedBirthDate percentIncrease = simulationBasedOn Reformed.calculatePensionNonSalaries percentIncrease 1980m

#load ".paket/load/net472/XPlot.GoogleCharts.fsx" // to evaluate in FSI .NET 4.7.2
//#load "../../.paket/load/netcoreapp2.2/XPlot.GoogleCharts.fsx" // not working in FSI, but necessary for autocomplete in IDE
open XPlot.GoogleCharts

let rateOfReturnSeries = "Taux de rendement (retraite annuelle/cotisations payées) selon les revenus (fixes) et par régime",
    "percent",
    [
        // "Régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].RateOfReturn)
        // "Régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].RateOfReturn)
        // "Régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
        // "Régime réformé des salariés", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
        ("Régime réformé des non salariés", ""), simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
        ("Régime SSI actuel", ""), simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
        ("Régime CIPAV actuel", ""), simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.GlobalRateOfReturn)
    ]

let cotisationsSeries = "Cotisations selon les revenus sur toute une carrière (revenus fixes) et par régime",
    "#,###€",
    [
        // "Régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].Cotisations)
        // "Régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].Cotisations)
        // "Régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
        // "Régime réformé des salariés", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
        ("Régime réformé des non salariés", ""), simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
        ("Régime SSI actuel", ""), simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
        ("Régime CIPAV actuel", ""), simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalCotisations)
    ]

let monthlyPensionSeries = "Retraite mensuelle nette selon les revenus sur toute une carrière (revenus fixes) et par régime",
    "#,###€",
    [
        // "Régime de base", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[0].MonthlyNetAmount)
        // "Régime AGIRC/ARRCO", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.ComposedOf.[1].MonthlyNetAmount)
        // "Régime général actuel", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyNetAmount)
        // "Régime réformé des salariés", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyNetAmount)
        ("Régime réformé des non salariés", ""), simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyNetAmount)
        ("Régime SSI actuel", ""), simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyNetAmount)
        ("Régime CIPAV actuel", ""), simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.TotalMonthlyNetAmount)
    ]

let replacementRateSeries = "Taux de remplacement selon les revenus sur toute une carrière (revenus fixes) et par régime",
    "percent",
    [
        // "Taux de remplacement régime général actuel, né en 1961", simulationCurrentBasedOnSameSalary 1961m |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        // "Régime général actuel, né en 1980", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        // "Régime général réformé, né en 1980", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        ("Régime réformé des non salariés", ""), simulationReformedNonSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        ("Régime SSI actuel", ""), simulationCurrentSsiBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        ("Régime CIPAV actuel", ""), simulationCurrentCipavBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    ]

let percentIncrease = 200m
let replacementRateSeriesPercentIncrease = sprintf "Taux de remplacement selon les revenus sur toute une carrière (revenus ayant augmenté linéairement de %d%%) et par régime" (percentIncrease |> int),
    "#,###€",
    [
        // "Taux de remplacement régime général actuel, né en 1961", simulationCurrentBasedOnSameSalary 1961m |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        // "Régime général actuel, né en 1980", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        // "Régime général réformé, né en 1980", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        ("Régime réformé des non salariés", ""), simulationReformedNonSalariesBasedOnPercentSalaryIncreaseFixedBirthDate percentIncrease |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        ("Régime SSI actuel", ""), simulationCurrentSsiBasedOnPercentSalaryIncreaseBirthDate percentIncrease |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
        ("Régime CIPAV actuel", ""), simulationCurrentCipavBasedOnPercentSalaryIncreaseBirthDate percentIncrease |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    ]

let ecartSsiReforme percentIncrease =
    Seq.zip
        (simulationReformedNonSalariesBasedOnPercentSalaryIncreaseFixedBirthDate percentIncrease)
        (simulationCurrentSsiBasedOnPercentSalaryIncreaseBirthDate percentIncrease)
    |> Seq.map (fun ((x1, y1), (x2, y2)) -> x1, y1.NetReplacementRate - y2.NetReplacementRate)
let replacementRateDiffSeriesDifferentPercentIncrease = "Baisse du taux de remplacement entre SSI et réforme selon les revenus sur toute une carrière (revenus ayant augmenté linéairement du pourcentage indiqué pour la courbe)", [
    // "Taux de remplacement régime général actuel, né en 1961", simulationCurrentBasedOnSameSalary 1961m |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    // "Régime général actuel, né en 1980", simulationCurrentBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    // "Régime général réformé, né en 1980", simulationReformedSalariesBasedOnSameSalaryFixedBirthDate |> Seq.map (fun (x, y) -> x, y.NetReplacementRate)
    ("fixe", "#E2A9F3"), ecartSsiReforme 0m
    ("+50%", "#D358F7"), ecartSsiReforme 50m
    ("+100%", "#BF00FF"), ecartSsiReforme 100m
    ("+200%", "#6A0888"), ecartSsiReforme 200m
]

let title, vAxisFormat, labels, seriesOptions, series =
    monthlyPensionSeries
    |> fun (title, vAxisFormat, seriesDef) ->
        let (labelsAndSeriesOptions, series) = seriesDef |> List.unzip
        let (labels, seriesOptions) = labelsAndSeriesOptions |> List.unzip
        let seriesOptions = seriesOptions |> List.map (function "" -> Series() | color -> Series(color = color)) |> List.toArray
        title, vAxisFormat, labels, seriesOptions, series

series
|> Chart.Combo
|> Chart.WithOptions(
    Options(
        vAxis = Axis(format = vAxisFormat),
        hAxis = Axis(viewWindow = ViewWindow(max = 25000)),
        aggregationTarget = "category",
        selectionMode = "multiple",
        tooltip = Tooltip(trigger = "selection"),
        series = seriesOptions))
|> Chart.WithTitle title
|> Chart.WithLabels labels
//|> Chart.WithXTitle "Salaire mensuel brut"
|> Chart.WithXTitle "Revenu (DSI) mensualisé"
|> Chart.WithLegend true
|> Chart.WithHeight 600
|> Chart.Show
