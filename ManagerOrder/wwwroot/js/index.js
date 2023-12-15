$(document).ready(function () {
    GetAllOrder(1);
});

//Get all data
function GetAllOrder(type) {
    $.ajax({
        url: '/Home/GetAllData',
        type: 'GET',
        dataType: 'json',
        data: {
            type: type
        },
        contentType: 'application/json',
        success: function (result) {
            //console.log(result);
            if (parseInt(result) != 0) {
                GetDataSummary(result);
                GetDataReport(result.Item3);
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Get tổng hợp chung
function GetDataSummary(dataSource) {
    //console.log('summary:', dataSource);
    var title1 = $('.total-product-card');
    var title2 = $('.total-money-import-card');
    var title3 = $('.total-money-revenue-card');
    var title4 = $('.total-money-interest-card');
    $(title1).text(new Intl.NumberFormat().format(dataSource.Item1.totalProduct));
    $(title2).text(new Intl.NumberFormat().format(dataSource.Item1.totalMoneyImport));
    $(title3).text(new Intl.NumberFormat().format(dataSource.Item2.totalRevenue));
    $(title4).text(new Intl.NumberFormat().format(dataSource.Item2.totalRevenue - dataSource.Item1.totalMoneyImport));
}

//Get biểu đồ báo cáo doanh thu
function GetDataReport(dataSource) {
    //console.log(dataSource);

    var options = {
        series: dataSource.data,
        chart: {
            type: 'bar',
            height: 350
        },
        plotOptions: {
            bar: {
                horizontal: false,
                columnWidth: '70%',
                endingShape: 'rounded'
            },
        },
        dataLabels: {
            enabled: false
        },
        xaxis: {
            categories: dataSource.categories,
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return `${new Intl.NumberFormat().format(val)} đ`
                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#reportsChart"), options);
    chart.render();
}

//Get danh sách sản phẩm
function GetDataProducts(dataSource) {
    //console.log(dataSource);

    var html = '';
    $.each(dataSource.data, (key, item) => {
        html += `<tr tabindex="0">
                    <td class="text-nowrap">${item.ProductCode}</td>
                    <td style="white-space:pre-line">${item.ProductName}</td>
                    <td class="text-nowrap">${item.UnitName}</td>
                    <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.QtyInventory)}</td>
                    <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.QtyImport)}</td>
                    <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.QtyExport)}</td>
                    <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.ProductImportPrice)}</td>
                    <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.WholesalePrice)}</td>
                </tr>`;
    });

    $('.tbody').html(html);
}