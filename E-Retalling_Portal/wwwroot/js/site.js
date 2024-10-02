//upload image function
//----------------------------------------------------------------------------------------
function showPreview(event, index) {
    if (event.target.files.length > 0) {
        let file = event.target.files[0];
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