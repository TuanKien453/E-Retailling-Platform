document.addEventListener("DOMContentLoaded", () => {
    // Load provinces initially
    fetchProvinces();

    // Function to fetch and display provinces
    function fetchProvinces() {
        fetch('/Address/getProvinces')
            .then(response => response.json())
            .then(data => {
                const provinceMap = {};
                data.forEach(province => {
                    provinceMap[province.provinceID] = province.provinceName;
                });

                // Update province cells
                const provinceCells = document.querySelectorAll('.province-cell');
                provinceCells.forEach(cell => {
                    const provinceId = cell.getAttribute('data-provinceid');
                    if (provinceMap[provinceId]) {
                        cell.textContent = provinceMap[provinceId];
                    }
                    fetchDistricts(provinceId);
                    console.log(provinceId);
                });
            })
            .catch(error => console.error('Error fetching provinces:', error));
    }

    function fetchDistricts(provinceId) {
        fetch(`/Address/getDistricts?provinceId=${provinceId}`)
            .then(response => response.json())
            .then(data => {
                const districtMap = {};
                data.forEach(district => {
                    districtMap[district.districtID] = district.districtName;
                });

                // Update district cells
                const districtCells = document.querySelectorAll('.district-cell');
                districtCells.forEach(cell => {
                    const districtId = cell.getAttribute('data-districtid');
                    if (districtMap[districtId]) {
                        cell.textContent = districtMap[districtId];
                    }
                    console.log(districtId);
                    fetchWards(districtId);  // Fetch wards based on districtId
                });
            })
            .catch(error => console.error('Error fetching districts:', error));
    }

    // Function to fetch and display wards
    function fetchWards(districtId) {
        fetch(`/Address/getWards?districtId=${districtId}`)
            .then(response => response.json())
            .then(data => {
                console.log(data);
                const wardMap = {};
                data.forEach(ward => {
                    wardMap[ward.wardCode] = ward.wardName;
                });

                // Update ward cells
                const wardCells = document.querySelectorAll('.ward-cell');
                wardCells.forEach(cell => {
                    const wardCode = cell.getAttribute('data-wardid');
                    if (wardMap[wardCode]) {
                        cell.textContent = wardMap[wardCode];
                    }
                });
            })
            .catch(error => console.error('Error fetching wards:', error));
    }
});
