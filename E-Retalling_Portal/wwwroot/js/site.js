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

function LoadImage(imageUrl, index, id) {
    let preview = document.getElementById("file-ip-" + index + "-preview");
    preview.src = imageUrl;
    preview.style.display = "block";

    let smallElement = document.querySelector('.small-ip-' + index);
    smallElement.textContent = "";

    let imgRemoveButton = document.querySelector('#button-remove-' + index);
    imgRemoveButton.classList.add('show');

    let input = document.getElementById("file-ip-" + index);
    input.name = 'img' + id;

    let dataContainer = document.querySelector('#file-data-container-' + index);
    let hiddenInput = document.createElement('input');
    hiddenInput.id = 'is-update-' + index;
    hiddenInput.type = 'hidden';
    hiddenInput.name = 'isUpdate' + id;
    hiddenInput.value = false;

    dataContainer.appendChild(hiddenInput);
}
// 
function showUpdatePreview(event, index) {
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

        let hiddenInput = document.querySelector('#is-update-' + index);
        if (hiddenInput != null) {
            hiddenInput.value = 'true';
        }
        
    }
}

function ImgUpdateRemove(index) {
    //set default img
    let preview = document.getElementById("file-ip-" + index + "-preview");
    let input = document.getElementById("file-ip-" + index);
    if (index == 1) {
        input.required = true;
    }
    preview.src = "/img/no-image-icon-23485.png";
    //remove data
    input.value = "";

    let smallElement = document.querySelector('.small-ip-' + index);
    smallElement.textContent = "Image";

    let imgRemoveButton = document.querySelector('#button-remove-' + index);
    imgRemoveButton.classList.remove('show');

    let hiddenInput = document.querySelector('#is-update-' + index);
    if (hiddenInput != null) {
        hiddenInput.value = 'true';
    }
    
}


