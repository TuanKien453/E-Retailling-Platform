//upload image function
//----------------------------------------------------------------------------------------
function showPreview(event, index) {
    const maxSize = 20 * 1024 * 1024;


    if (event.target.files.length > 0) {
        let file = event.target.files[0];
        if (file.size > maxSize) {
            alert('File size exceeds the limit of 20MB.');
            event.target.value = '';
            return;
        } 
        let src = URL.createObjectURL(file);
        let preview = document.getElementById("file-ip-" + index + "-preview");
        preview.src = src;
        preview.style.display = "block";

        let fileName = file.name;
        let smallElement = document.querySelector('.small-ip-' + index);
        smallElement.textContent = fileName;

        let imgRemoveButton = document.querySelector('#button-remove-' + index);
        imgRemoveButton.classList.add('show');
    }
}

// Function to remove image
function ImgRemove(index) {
    //set default img
    let preview = document.getElementById("file-ip-" + index + "-preview");
    let input = document.getElementById("file-ip-" + index);
    preview.src = "/img/no-image-icon-23485.png";
    //remove data
    input.value = "";

    let smallElement = document.querySelector('.small-ip-' + index);
    smallElement.textContent = "Image";

    let imgRemoveButton = document.querySelector('#button-remove-' + index);
    imgRemoveButton.classList.remove('show');
}
//----------------------------------------------------------------------------------------


//Filter by price
var slider = new Slider('#ex2', {
    tooltip: 'hide',
    range: true
});

function updatePriceLabels(value) {
    document.getElementById('minPrice').textContent = '$ ' + value[0];
    if (value[1] === 1000) {
        document.getElementById('maxPrice').textContent = '$ 1000+';
    } else {
        document.getElementById('maxPrice').textContent = '$ ' + value[1];
    }
    document.getElementById('minPriceInput').value = value[0];
    document.getElementById('maxPriceInput').value = value[1];
}


slider.on('slide', function (value) {
    updatePriceLabels(value);
});

slider.on('change', function (event) {
    var value = event.newValue;
    updatePriceLabels(value);
});
//-----------------------------------------------------------------

