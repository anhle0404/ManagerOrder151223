$(document).ready(function () {
    GetAllOrder();
})

//Sự kiện get danh sách đơn hàng
function GetAllOrder() {
    $.ajax({
        url: '/HistoryOrder/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            isApproved: parseInt($('#isapproved_history_order').val()),
            keyword: $('#keyword_history_order').val(),
        },
        contentType: 'application/json',
        success: function (result) {
            if (parseInt(result) != 0) {


                var html = '';
                var htmlCard = '';
                $.each(result, function (key, item) {
                    var bgStatus = item.IsApproved == 1 ? 'bg-success text-white' : 'bg-warning text-dark';
                    var colorStatus = item.IsApproved == 1 ? 'text-white' : '';
                    html += `<tr tabindex="0">
                            <td class="text-nowrap sticky-left-table ${bgStatus}" onclick="return GetOrderDetail(${item.Id});">${item.OrderCode}</td>
                            <td style="white-space:pre-line">${item.CustomerName}</td>
                            <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.TotalIntoMoney)} vnđ</td>
                            <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.CustomerPayment)} vnđ</td>
                            <td class="text-nowrap text-end">${new Intl.NumberFormat().format(item.MoneyOwedCustomer)} vnđ</td>
                            <td class="text-nowrap">${item.CreatedDate}</td>
                        </tr>`;

                    htmlCard += `<div class="card mb-3">
                                <div class="card-hearder border-bottom p-2 justify-content-between ${bgStatus}">
                                    <h5 class="card-title text-uppercase p-0 m-0 ${colorStatus}" onclick="return GetOrderDetail(${item.Id});">${item.OrderCode}</h5>
                                    ${item.CreatedDate}
                                </div>
                                <div class="card-body p-2">
                                    ${item.CustomerName}
                                </div>
                                <div class="card-footer d-flex justify-content-between flex-wrap p-2">
                                    <small>Tổng tiền: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.TotalIntoMoney)} vnđ</span></small>
                                    <small>Tiền khách trả: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.CustomerPayment)} vnđ</span></small>
                                    <small>Tiền khách nợ: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.MoneyOwedCustomer)} vnđ</span></small>
                                </div>
                            </div>`;
                });

                $('.tbody').html(html);
                $('.list-card').html(htmlCard);
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Sự kiện get chi tiết đơn hàng
function GetOrderDetail(idOrder) {

    $.ajax({
        url: '/HistoryOrderDetailRepo/GetAll',
        type: 'GET',
        dataType: 'json',
        data: {
            historyOrderId: idOrder
        },
        contentType: 'application/json',
        success: function (result) {
            //console.log(result);
            if (parseInt(result.status) == 1) {

                var htmlbody = '';

                var htmlfooter = `<p class="text-dark">Tổng tiền: <span class="text-danger">${new Intl.NumberFormat().format(result.order.TotalIntoMoney)} vnđ</span></p>
                                    <button type="button" class="btn btn-primary btn-sm" onclick="return onClickDelivered(${result.order.Id}, ${result.order.TotalIntoMoney});">Giao hàng</button>`;
                if (result.order.IsApproved == 1) {
                    htmlfooter = `<p class="text-dark">Tổng tiền: <span class="text-danger">${new Intl.NumberFormat().format(result.order.TotalIntoMoney)} vnđ</span></p>
                                    <button type="button" class="btn btn-success btn-sm"> Đã giao hàng</button>`;
                }

                $.each(result.detail, function (key, item) {
                    htmlbody += `<div class="card border-top border-primary border-2 m-1 p-0">
                                <div class="card-body p-1">
                                    <h5 class="card-title p-0 m-0">${item.ProductCode}</h5>
                                    ${item.ProductName}
                                </div>
                                <div class="card-footer d-flex justify-content-between flex-wrap p-2">
                                    <small>Đơn giá: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.Price)} vnđ</span></small>
                                    <small>Số lượng: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.Quantity)}</span></small>
                                    <small>Thành tiền: <span class="text-dark fw-bold">${new Intl.NumberFormat().format(item.TotalPrice)} vnđ</span></small>
                                </div>
                            </div>`;
                });

                $('.modal-title').text(result.order.OrderCode);
                $('.list-group').html(htmlbody);
                $('.modal-footer').html(htmlfooter);

                $('#basicModal').modal('show');
            } else {
                alert(result.message);
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    })
}

//Sự kiện click giao hàng
function onClickDelivered(id, totalmoney) {
    let money = prompt("Vui lòng nhập số tiền khách trả để xác nhận giao hàng:", totalmoney);
    if (parseFloat(money) > 0) {
        var obj = {
            Id: id,
            CustomerPayment: parseFloat(money)
        };

        $.ajax({
            url: '/HistoryOrder/Update',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(obj),
            contentType: 'application/json',
            success: function (result) {
                if (parseInt(result) > 0) {
                    GetAllOrder();
                    $('#basicModal').modal('hide');
                }
            },
            error: function (err) {
                alert(err.responseText);
            }
        })

    }
}