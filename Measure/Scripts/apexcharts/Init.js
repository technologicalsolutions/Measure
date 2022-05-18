function CreateGroup(Grupos) {
    var Parent = document.getElementById("GraphicGroup");
    $.each(Grupos, function (pos, item) {
        var child = document.createElement("div");
        child.classList.add("col-12");
        child.classList.add("col-sm-4");

        var element = document.createElement("div");
        element.classList.add("grupo");
        element.setAttribute("data-name", item);

        child.appendChild(element);
        Parent.appendChild(child);
    })
}

function ComboMixto(Series, Labels, Colors, querySelector) {
    var SWidth = Array();

    $.each(Series, function () {
        SWidth.push(2);
    })

    var options = {
        series: Series,
        chart: {
            height: 350,
            type: 'line',
            stacked: false,
        },
        stroke: {
            width: SWidth,
            curve: 'smooth'
        },
        plotOptions: {
            bar: {
                borderRadius: 10,
                columnWidth: '50%'
            }
        },
        colors: Colors,
        fill: {
            opacity: [0.85, 0.25, 1],
            gradient: {
                inverseColors: false,
                shade: 'light',
                type: "vertical",
                opacityFrom: 0.85,
                opacityTo: 0.55,
                stops: [0, 100, 100, 100]
            }
        },
        labels: Labels,
        markers: {
            size: 0
        },
        yaxis: {
            title: {
                text: 'Calificación',
            },
            min: 0,
            max: 10,
        },
        tooltip: {
            shared: true,
            intersect: false,
            y: {
                formatter: function (y) {
                    if (typeof y !== "undefined") {
                        return y.toFixed(0) + " Calificación";
                    }
                    return y;

                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector(querySelector), options);
    chart.render();
}

function ColumnasApiladas(Series, Categories, Colors, querySelector) {
    var options = {
        series: Series,
        chart: {
            type: 'bar',
            height: 450,
            stacked: true,
            toolbar: {
                show: true
            },
            zoom: {
                enabled: true
            }
        },
        responsive: [{
            breakpoint: 480,
            options: {
                legend: {
                    position: 'bottom',
                    offsetX: -10,
                    offsetY: 0
                }
            }
        }],
        plotOptions: {
            bar: {
                horizontal: false,
                borderRadius: 10
            },
        },
        xaxis: {
            categories: Categories,
        },
        legend: {
            position: 'right',
            offsetY: 40
        },
        fill: {
            opacity: 1
        }
    };

    if (Colors != null) {
        options['colors'] = Colors;
    }
    var chart = new ApexCharts(document.querySelector(querySelector), options);
    chart.render();
}

function ColumnasBasicas(Titulo, Nombre, Series, Categories, Colors, querySelector) {
    var options = {
        title: {
            text: Titulo,
        },
        series: [{
            name: Nombre,
            data: Series
        }],
        chart: {
            height: 350,
            type: 'bar',
        },
        plotOptions: {
            bar: {
                horizontal: false,
                distributed: true,
                borderRadius: 10
            },
        },
        xaxis: {
            categories: Categories,
            labels: {
                style: {
                    fontSize: '12px'
                }
            }
        },
        dataLabels: {
            enabled: false
        },
        yaxis: {
            min: 0,
            max: 10,
        },
    };

    if (Colors != null) {
        options['colors'] = Colors;
    }
    var chart = new ApexCharts(document.querySelector(querySelector), options);
    chart.render();

}

function ColumnasBasicasGeneral(Titulo, Series, Categories, Colors, querySelector) {
    var options = {
        title: {
            text: Titulo,
        },
        series: [
            {
                name: 'Porcentaje',
                data: Series
            }
        ],
        chart: {
            height: 450,
            type: 'bar',
            zoom: {
                enabled: true
            }
        },
        plotOptions: {
            bar: {
                borderRadius: 10,
                horizontal: false,
                distributed: true,
            }
        },
        yaxis: {
            min: 0,
            max: 10,
        },
        xaxis: {
            categories: Categories
        }
    };

    if (Colors != null) {
        options['colors'] = Colors;
    }
    var chart = new ApexCharts(document.querySelector(querySelector), options);
    chart.render();
}

function AreaPolar(Titulo, Series, Labels, Colors) {
    var options = {
        title: {
            text: Titulo,
            align: 'center',
        },
        series: Series,
        chart: {
            type: 'polarArea',
            height: 450,
        },
        labels: Labels,
        fill: {
            opacity: 0.8
        },
        yaxis: {
            min: 0,
            max: 10,
        },
        responsive: [{
            breakpoint: 450,
            options: {
                chart: {
                    width: 200
                },
                legend: {
                    position: 'bottom'
                }
            }
        }]
    };

    if (Colors != null) {
        options['colors'] = Colors;
    }

    var querySelector = "[data-name='" + Titulo + "']";
    var chart = new ApexCharts(document.querySelector(querySelector), options);
    chart.render();
}