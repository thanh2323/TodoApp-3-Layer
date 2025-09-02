// Auto-hide alerts after 5 seconds
$(document).ready(function () {
    $('.alert').delay(5000).fadeOut('slow');

    // Add CSRF token to all AJAX requests
    $.ajaxSetup({
        beforeSend: function (xhr, settings) {
            if (settings.type !== 'GET' && settings.type !== 'HEAD') {
                var token = $('input[name="__RequestVerificationToken"]').val();
                if (token) {
                    xhr.setRequestHeader('RequestVerificationToken', token);
                }
            }
        }
    });
});

// Confirmation dialogs
function confirmDelete(message) {
    return confirm(message || 'Are you sure you want to delete this item?');
}

// Loading spinner functions
function showLoading() {
    $('body').append('<div id="loading-overlay" class="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center" style="background: rgba(0,0,0,0.5); z-index: 9999;"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
}

function hideLoading() {
    $('#loading-overlay').remove();
}

// Form validation enhancement
function highlightValidationErrors() {
    $('.field-validation-error').each(function () {
        $(this).closest('.form-group, .mb-3').find('.form-control').addClass('is-invalid');
    });
}

// Initialize on page load
$(document).ready(function () {
    highlightValidationErrors();

    // Remove validation errors on input change
    $('.form-control').on('input change', function () {
        $(this).removeClass('is-invalid');
        $(this).closest('.form-group, .mb-3').find('.field-validation-error').hide();
    });
});