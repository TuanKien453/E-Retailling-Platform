let subtotal = 0;

function updateTotal(input, price, totalElementId, itemId, quantityInStock, isProductJs) {
    let quantity = parseInt(input.value);
    let productPrice = parseFloat(price);
    let quantityInStockNow = parseInt(quantityInStock);
    let isProductUpdate = isProductJs;
    let itemIdUpdate = parseInt(itemId);

    let quantityModal = new bootstrap.Modal(document.getElementById('quantityModal'));
    let modalMessage = document.getElementById('quantityModalMessage');

    // Validate quantity input
    if (isNaN(quantity) || quantity <= 0) {
        modalMessage.innerText = "Please input a positive integer number.";
        input.value = 1;
        quantity = 1;
        quantityModal.show();
    } else if (quantity > quantityInStockNow) {
        modalMessage.innerText = "This product only has " + quantityInStockNow + " available in stock.";
        input.value = 1;
        quantity = 1;
        quantityModal.show();
    }

   
    let total = quantity * productPrice;
    input.value = quantity;

    document.getElementById(totalElementId).innerText = total.toFixed(2) + " VND";

    // Send AJAX request to update cart
    $.ajax({
        type: 'POST',
        url: '/Cart/UpdateFromCart',
        data: {
            itemId: itemIdUpdate,
            quantity: quantity,
            isProduct: isProductUpdate
        },
    });

    updateSubtotal();
}

document.addEventListener('DOMContentLoaded', function () {
    updateSubtotal();

    const quantityInputs = document.querySelectorAll('input[type="number"]');
    quantityInputs.forEach(input => {
        input.addEventListener('blur', function () {
            updateTotal(this, this.getAttribute('data-price'), this.getAttribute('data-total-id'), this.getAttribute('data-product-id'), this.getAttribute('data-quantityInStock'), this.getAttribute('data-isProduct'));
        });
    });
});

function updateSubtotal() {
    let subtotal = 0;

    // Select all total elements for products and product items
    const totalElements = document.querySelectorAll('h5[id^="total-"]');

    totalElements.forEach((element) => {
        const itemId = element.id.split("-")[2];
        const item = element.id.split("-")[1];
        const checkbox = document.getElementById('check-' + item + '-' + itemId);
        if (checkbox.checked) {
            const totalText = element.innerText.replace('VND', '');
            subtotal += parseFloat(totalText);
        }
    });

    document.getElementById('subtotal').innerText = subtotal.toFixed(2) + " VND";
}


let itemIdToDelete = null;
let isProductToDelete = null;

function showDeleteModal(itemId, productName, isProduct) {
    itemIdToDelete = parseInt(itemId);
    isProductToDelete = isProduct;
    document.getElementById('modalProductName').innerText = productName;

    // Use Bootstrap's Modal API to show the modal
    const modalElement = new bootstrap.Modal(document.getElementById('deleteModal'));
    modalElement.show();
}

// Event listener for the confirm button in the modal
document.getElementById('confirmDeleteButton').addEventListener('click', function () {
    if (itemIdToDelete) {
        deleteFromCart(itemIdToDelete, isProductToDelete);
    }
});


function deleteFromCart(itemId, isProduct) {
    $.ajax({
        type: 'POST',
        url: '/Cart/DeleteFromCart',
        data: { itemId: itemId, isProduct: isProduct },
        success: function (response) {
            window.location.reload();
        },
        error: function (xhr, status, error) {
            console.error("An error occurred while trying to delete the item. This item maybe not existed anymore", error);
            alert("This item maybe not existed anymore. Please refresh page");
        }
    });
}
document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("checkoutButton").addEventListener("click", function () {
        const loginStatus = document.getElementById("loginStatus").value;
        if (loginStatus === '') {
            location.href = 'Login';
            return;
        }
        const serverResponseDiv = document.getElementById("serverresponse");
        if (serverResponseDiv.style.display !== "none") {
            $('#noItemsModal .modal-title').text('Error while checkout');
            $('#noItemsModal .modal-body p').text('Please check your order is valid');
            $('#noItemsModal').modal('show');
            return;
        }
        const districtId = document.getElementById("quan").value;
        const wardId = document.getElementById("phuong").value;
        const address = document.getElementById("address").value.trim();
        if (districtId === "0" || wardId === "0" || address === '' || address.length < 5) {
            $('#noItemsModal .modal-title').text('No Address Selected');
            $('#noItemsModal .modal-body p').text('Please select your address.');
            $('#noItemsModal').modal('show');
            return;
        }
        const paymentMethod = document.getElementById("paymethod").value;

        if (paymentMethod === "0") {
            $('#noItemsModal .modal-title').text('No Payment Method Selected');
            $('#noItemsModal .modal-body p').text('Please select a payment method.');
            $('#noItemsModal').modal('show');
            return;
        }


        let selectedItemsString = Array.from(document.querySelectorAll(".product-checkbox:checked")).map(checkbox => {
            const id = checkbox.getAttribute("data-id");
            const quantity = checkbox.getAttribute("data-quantity");

            return `${id}:${quantity}`;
        }).join(",");

        if (selectedItemsString === "") {
            $('#noItemsModal .modal-title').text('No Items Selected');
            $('#noItemsModal .modal-body p').text('Please select at least one item to proceed to checkout.');
            $('#noItemsModal').modal('show');
            return;
        }


        sessionStorage.setItem('selectedItems', selectedItemsString);

        const form = document.getElementById("checkoutForm")
        form.method = "Post";
        form.action = "/CheckOut/CreatePaymentUrl";
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "cartItems";
        input.value = selectedItemsString;
        form.appendChild(input);

        const paymentInput = document.createElement("input");
        paymentInput.type = "hidden";
        paymentInput.name = "paymentMethod";
        paymentInput.value = paymentMethod;
        form.appendChild(paymentInput);
        form.submit();
    });
});

function calculatefee() {
    var selectedWardCode = $("#phuong").val();
    var selectedDistrict = $("#quan").val();
    var products = [];
    var totalFee = 0.00;


    $('input.product-checkbox:checked').each(function () {
        var productId = $(this).val();
        var quantity = $(this).closest('.card-body').find('input[type="number"]').val();

        if (productId && quantity) {
            products.push({
                productId: parseInt(productId),
                quantity: parseInt(quantity)
            });
        }
    });

    if (!selectedWardCode || selectedWardCode === "0" || !selectedDistrict || selectedDistrict === "0" || products.length === 0) {
        $("#shipfee").text("0.00" + ' VND');
        $("#serverresponse").hide();
        return;
    }
    $("#serverresponse").hide();
    var requests = products.map(function (product) {
        return $.ajax({
            url: '/CheckOut/CalculateShippingFee',
            type: 'POST',
            data: {
                productId: product.productId,
                quantity: product.quantity,
                toDistrcitId: parseInt(selectedDistrict),
                toWardCode: selectedWardCode
            }
        }).then(function (response) {
            console.log("Shipping fee calculated for product ID " + product.productId + ":", response);
            return response.fee || 0;
        }).catch(function (xhr) {
            var errorMessage = xhr.responseText || "An error occurred while calculating shipping fee.";
            $("#serverresponse").text(errorMessage).show();
            return 0;
        });
    });


    Promise.all(requests).then(function (fees) {
        totalFee = fees.reduce(function (sum, fee) {
            return sum + fee;
        }, 0);

        console.log(totalFee);
        $("#shipfee").text(totalFee.toFixed(2) + ' VND');
        calculateTotal();
    });
}

function calculateTotal() {
    const subtotalElement = document.getElementById("subtotal");
    const shipfeeElement = document.getElementById("shipfee");
    const totalElement = document.getElementById("total");

    const subtotal = parseFloat(subtotalElement.textContent.replace(" VND", "").replace(",", "")) || 0;
    const shippingFee = parseFloat(shipfeeElement.textContent.replace(" VND", "").replace(",", "")) || 0;

    const total = subtotal + shippingFee;

    totalElement.textContent = total.toFixed(2) + " VND";
}

document.addEventListener('DOMContentLoaded', function () {
    const quantityInputs = document.querySelectorAll('input[type="number"]');
    quantityInputs.forEach(input => {
        input.addEventListener('input', function () {
            const newQuantity = this.value;

            const productId = this.getAttribute('data-product-id'); 
            let checkbox;

            if (this.getAttribute('data-isProduct') === 'true') {
                checkbox = document.querySelector(`#check-product-${productId}`);
            } else {
                checkbox = document.querySelector(`#check-productItem-${productId}`);
            }

            if (checkbox) {
                checkbox.setAttribute('data-quantity', newQuantity);
            }
        });
    });
});
