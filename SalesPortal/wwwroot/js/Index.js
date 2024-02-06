
let _filter = {
    startDate: null,
    endDate: null,
    selectedCountry: null,
    selectedRegion: null,
    selectedCity: null,
    pageSize: 5,
    pageNumber: 1,
}

var TotalPages = 0;

// Function to get total pages
function getTotalPages(startDate, endDate, selectedCountry, selectedRegion, selectedCity, pageSize) {
    $.ajax({
        url: '/Home/GetTotalPages',
        type: 'GET',
        data: {
            startDate: startDate,
            endDate: endDate,
            selectedCountry: selectedCountry,
            selectedRegion: selectedRegion,
            selectedCity: selectedCity,
            pageSize: pageSize },
        success: function (data) {
            TotalPages = data.TotalPages;
        },
        error: function (error) {
            console.error('Error getting total pages:', error);
        }
    });
}

function loadSalesData(startDate, endDate, selectedCountry, selectedRegion, selectedCity, pageSize, pageNumber) {
    $.ajax({
        url: '/Home/GetSales', 
        type: 'GET',
        data: {
            startDate: startDate,
            endDate: endDate,
            selectedCountry: selectedCountry,
            selectedRegion: selectedRegion,
            selectedCity: selectedCity,
            pageSize: pageSize,
            pageNumber: pageNumber
        },
        success: function (responseJson) {
            if (responseJson.length > 0) {
                // Clean table first
                $("#tblSales tbody").html("");

                // Add information to table
                responseJson.forEach((sale) => {
                    // Create a new row
                    var row = $("<tr>");

                    // Append cells to the row
                    row.append(
                        $("<td>").text(sale.CustomerName),
                        $("<td>").text(sale.CountryName),
                        $("<td>").text(sale.RegionName),
                        $("<td>").text(sale.CityName),
                        $("<td>").text(sale.SaleDateTime),
                        $("<td>").text(sale.ProductName).css("color", getQuantityColor(sale.Quantity)), // Apply color based on quantity
                        $("<td>").text(sale.Quantity)
                    );

                    // Append the row to the table
                    $("#tblSales tbody").append(row);
                });

                updatePagination(pageNumber);
            }
        },
        error: function (error) {
            console.error('Error getting sales:', error);
        }
    });
}

// Function to determine color based on quantity
function getQuantityColor(quantity) {
    if (quantity > 100) {
        return "green"; // Green for quantity greater than 100
    } else if (quantity <= 50) {
        return "red"; // Red for quantity less than or equal to 50
    } else {
        return ""; // Default color
    }
}

function updatePagination(currentPage) {
    var paginationElement = $('.pagination');
    var prevButtonParent = paginationElement.find('.prev').parent();
    var nextButtonParent = paginationElement.find('.next').parent();
    var firstButtonParent = paginationElement.find('.first').parent();

    if (currentPage === 1) {
        prevButtonParent.addClass('disabled');
        firstButtonParent.addClass('disabled');
    } else {
        prevButtonParent.removeClass('disabled');
        firstButtonParent.removeClass('disabled');
    }

    if (currentPage === TotalPages) {
        nextButtonParent.addClass('disabled');
    } else {
        nextButtonParent.removeClass('disabled');
    }

    // Update the current page number display
    $('#currentPage').text(currentPage);
}



function OpenSalesModal() {

    //show modal
    $("#modalSales").modal("show");
}

function OpenFiltersModal() {
    $("startDate").val(_filter.startDate);
    $("endDate").val(_filter.endDate);
    $("ddlCountryFilter").val(_filter.selectedCountry);

    //show modal
    $("#modalFilters").modal("show");
}

//event when add sales button clicked
$(document).on("click", ".button-new-sales", function () {
    
    OpenSalesModal();
})

$(document).on("click", ".btn-save-sale", function () {

    var sale = {
        SaleID: 0,
        CustomerName: document.getElementById('txtCustomerName').value,
        CountryID: parseInt(document.getElementById('ddlCountry').value),
        RegionID: parseInt(document.getElementById('ddlRegion').value),
        CityID: parseInt(document.getElementById('ddlCity').value),
        SaleDateTime: document.getElementById('txtSaleDateTime').value ?? new Date().toDateString(),
        ProductID: parseInt(document.getElementById('ddlProduct').value),
        Quantity: parseInt(document.getElementById('txtQuantity').value),
    };

    fetch('/Home/SaveSale', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(sale),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(result => {
            $("#modalSales").modal("hide");
            alert("Sales data added successfully.");
            loadSalesData(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize, 1);
        })
        .catch(error => {
            console.error('Error saving sale:', error);
        });
})

//event when filters button clicked
$(document).on("click", ".button-filter", function () {
    //reset client model data
    _filter.startDate = null;
    _filter.endDate = null;
    _filter.selectedCountry = null;
    _filter.selectedRegion = null;
    _filter.selectedCity = null;

    OpenFiltersModal();
})

//event when apply filter button clicked
$(document).on("click", ".btn-apply-filter", function () {
    var selectedCountries = $("#ddlCountryFilter option:selected").filter(function () {
        return $(this).val() > 0;
    }).map(function () {
        return $(this).text();
    }).get();

    var selectedRegions = $("#ddlRegionFilter option:selected").filter(function () {
        return $(this).val() > 0;
    }).map(function () {
        return $(this).text();
    }).get();

    var selectedCities = $("#ddlCityFilter option:selected").filter(function () {
        return $(this).val() > 0;
    }).map(function () {
        return $(this).text();
    }).get();

    // Combine selected values into comma-separated strings
    _filter.selectedCountry = selectedCountries ? selectedCountries.join(',') : null;
    _filter.selectedRegion = selectedRegions ? selectedRegions.join(',') : null;
    _filter.selectedCity = selectedCities ? selectedCities.join(',') : null;

    // Get selected date values
    _filter.startDate = $("#startDate").val() ? $("#startDate").val() : null;
    _filter.endDate = $("#endDate").val() ? $("#endDate").val() : null;

    //update TotalPages
    getTotalPages(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize);
    // Call loadSalesData with the selected filter values
    loadSalesData(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize, 1);

    $('#modalFilters').modal('hide');
})

//event when clear filter button clicked
$(document).on("click", ".btn-clear-filter", function () {
    //reset client model data
    _filter.startDate = null;
    _filter.endDate = null;
    _filter.selectedCountry = null;
    _filter.selectedRegion = null;
    _filter.selectedCity = null;

    loadSalesData(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize, 1);

    $('#modalFilters').modal('hide');
})

// Handle pagination clicks
$(document).on('click', '.page-link', function () {
    var pageNumber = parseInt($('#currentPage').text()); // Get the current page number
    if ($(this).attr('aria-label') === 'Previous') {
        loadSalesData(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize, pageNumber - 1);
    } else if ($(this).attr('aria-label') === 'Next') {
        loadSalesData(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize, pageNumber + 1);
    } else if ($(this).attr('aria-label') === 'First') {
        loadSalesData(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize, 1);
    }
});


// Function to populate dropdown based on the received data
function populateDropdown(ddlId, data) {
    const ddl = $(`#${ddlId}`);
    ddl.html('<option selected>Open this select menu</option>');
    if (ddlId == 'ddlCountryFilter') {
        ddl.html('');
    }

    // Declare valueProp and textProp
    let valueProp, textProp;

    // Determine the value and text properties based on ddlId
    switch (ddlId) {
        case 'ddlProduct':
            valueProp = 'ProductID';
            textProp = 'ProductName';
            break;
        case 'ddlCountry':
        case 'ddlCountryFilter':
            valueProp = 'CountryID';
            textProp = 'CountryName';
            break;
        case 'ddlRegion':
        case 'ddlRegionFilter':
            valueProp = 'RegionID';
            textProp = 'RegionName';
            break;
        case 'ddlCity':
        case 'ddlCityFilter':
            valueProp = 'CityID';
            textProp = 'CityName';
            break;
        default:
            valueProp = '';
            textProp = '';
    }

    // Populate the dropdown
    $.each(data, function (index, item) {
        ddl.append($('<option>', {
            value: item[valueProp],
            text: item[textProp],
        }));
    });
}

// Function to make an AJAX call and populate the dropdown
function populateDropdownFromServer(ddlId, url) {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            populateDropdown(ddlId, data);
        },
        error: function (error) {
            console.error(`Error fetching data from ${url}`, error);
        }
    });
}

$(document).ready(function () {
    
    getTotalPages(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize);

    //show sales data
    loadSalesData(_filter.startDate, _filter.endDate, _filter.selectedCountry, _filter.selectedRegion, _filter.selectedCity, _filter.pageSize, _filter.pageNumber);

    //populate ddls
    $.ajax({
        url: '/Home/GetCountries',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            populateDropdown('ddlCountry', data);
        },
        error: function (error) {
            console.error(`Error fetching data from /Home/GetCountries`, error);
        }
    });
    $.ajax({
        url: '/Home/GetProducts',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            populateDropdown('ddlProduct', data);
        },
        error: function (error) {
            console.error(`Error fetching data from /Home/GetCountries`, error);
        }
    });


    // Event listener for ddlCountry
    $('#ddlCountry').on('change', function () {
        const selectedCountryId = parseInt($(this).val());
        populateDropdownFromServer('ddlRegion', `/Home/GetRegions?countryId=${selectedCountryId}`);
    });

    // Event listener for ddlRegion
    $('#ddlRegion').on('change', function () {
        const selectedRegionId = parseInt($(this).val());
        populateDropdownFromServer('ddlCity', `/Home/GetCities?regionId=${selectedRegionId}`);
    });

    //For Filters 
    $.ajax({
        url: '/Home/GetCountries',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            populateDropdown('ddlCountryFilter', data);
        },
        error: function (error) {
            console.error(`Error fetching data from /Home/GetCountries`, error);
        }
    });
    // Event listener for ddlCountryFilter (multiple select)
    $('#ddlCountryFilter').on('change', function () {
        const selectedCountryIds = $(this).val();

        // Check if any country is selected
        if (selectedCountryIds && selectedCountryIds.length > 0) {
            const ajaxRequests = [];
            
            // Make an AJAX request for each selected country
            selectedCountryIds.forEach(function (selectedCountryId) {
                const url = `/Home/GetRegions?countryId=${parseInt(selectedCountryId)}`;

                // Push the AJAX request to the array
                ajaxRequests.push($.ajax({
                    url: url,
                    type: 'GET',
                    dataType: 'json'
                }));
            });

            // Use $.when to handle multiple asynchronous requests
            $.when.apply($, ajaxRequests).done(function () {
                // Collect data from the AJAX responses
                const results = [];
                for (let i = 0; i < arguments.length; i++) {
                    let responsResult = arguments[i][0];
                    if (selectedCountryIds.length === 1) {
                        responsResult = arguments[0]; //if only one request, the responsResult will not wrapped with array
                    }
                    for (let j = 0; j < responsResult.length; j++) {
                        results.push(responsResult[j]);
                    }
                    if (selectedCountryIds.length === 1) {
                        break; //if only one request, the responsResult will not wrapped with array
                    }
                }
                // Populate the dropdown with the collected data
                populateDropdown('ddlRegionFilter', results);
            }).fail(function () {
                console.error('Error fetching data from one or more URLs');
            });
        } else {
            $('#ddlRegionFilter').empty();
        }
    });

    $('#ddlRegionFilter').on('change', function () {
        const selectedRegionId = parseInt($(this).val());
        populateDropdownFromServer('ddlCityFilter', `/Home/GetCities?regionId=${selectedRegionId}`);
    });
})