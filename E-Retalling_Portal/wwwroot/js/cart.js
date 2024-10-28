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
    document.getElementById(totalElementId).innerText = "$" + total.toFixed(2);

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
            const totalText = element.innerText.replace('$', '');
            subtotal += parseFloat(totalText);
        }
    });

    document.getElementById('subtotal').innerText = "$" + subtotal.toFixed(2);
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
        let selectedItemsString = Array.from(document.querySelectorAll(".product-checkbox:checked")).map(checkbox => {
            const id = checkbox.getAttribute("data-id");
            const quantity = checkbox.getAttribute("data-quantity");

            return `${id}:${quantity}`;
        }).join(",");

        // Kiểm tra xem có sản phẩm nào được chọn không
        if (selectedItemsString === "") {
            // Hiển thị modal thông báo
            $('#noItemsModal').modal('show');
        } else {
            // Lưu selectedItemsString vào sessionStorage
            sessionStorage.setItem('selectedItems', selectedItemsString);
            // Chuyển hướng đến CreatePaymentUrl
            window.location.href = '/CheckOut/CreatePaymentUrl';
        }
    });
});




