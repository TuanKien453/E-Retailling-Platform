
let subtotal = 0;

function updateTotal(input, price, totalElementId, productItemId, quantityInStock) {
    let quantity = parseInt(input.value);
    let productPrice = parseFloat(price);
    let quantityInStockNow = parseInt(quantityInStock);


    if (isNaN(quantity)) {
        alert("Please input an positive integer number");
        input.value = 1;
        quantity = 1;
    }
    else if (quantity > quantityInStockNow) {
        alert("This product just has " + quantityInStock + " availables in stock");
        input.value = 1;
        quantity = 1;
    }

    let total = quantity * productPrice;

    document.getElementById(totalElementId).innerText = "$" + total.toFixed(2);

    $.ajax({
        type: 'POST',
        url: '/Cart/UpdateFromCart',
        data: {
            productItemId: productItemId,
            quantity: quantity
        },
        success: function (response) {
            if (response.success) {
                console.log("Update quantity successfully");
                updateSubtotal();
            } else {
                console.error("Update quantity unsuccessfully");
            }
        },
        error: function () {
            console.error("Error existing when update");
        }
    });

    updateSubtotal();
}

document.addEventListener('DOMContentLoaded', function () {
    updateSubtotal();

    const quantityInputs = document.querySelectorAll('input[type="number"]');
    quantityInputs.forEach(input => {
        input.addEventListener('blur', function () {
            updateTotal(this, this.getAttribute('data-price'), this.getAttribute('data-total-id'), this.getAttribute('data-product-id'), this.getAttribute('data-quantityInStock'));
        });
    });
});

function updateSubtotal() {
    subtotal = 0;

    const totalElements = document.querySelectorAll('h5[id^="total-"]'); // element has <h5> and id has total-
    totalElements.forEach((element) => {
        const totalText = element.innerText.replace('$', '');
        subtotal += parseFloat(totalText);
    });

    document.getElementById('subtotal').innerText = "$" + subtotal.toFixed(2);

    calculateTotal();
}

function calculateTotal() {
    const subtotalText = document.getElementById('subtotal').innerText.replace('$', '');
    const subtotalValue = parseFloat(subtotalText);
    const shippingCharge = 25;

    let total = subtotalValue + shippingCharge;
    if (total <= 0) total = 0;
    document.getElementById('total').innerText = "$" + total.toFixed(2);
}

function deleteFromCart(productItemId) {
    if (confirm("Are you sure you want to remove this item from your cart?")) {
        $.ajax({
            type: 'POST',
            url: '/Cart/DeleteFromCart',
            data: { productItemId: productItemId },
            success: function (response) {
                window.location.reload();
            },
            error: function (xhr, status, error) {
                console.error("An error occurred while trying to delete the item:", error);
                alert("An error occurred. Please try again. Error: " + error);
            }
        });
    }
}
