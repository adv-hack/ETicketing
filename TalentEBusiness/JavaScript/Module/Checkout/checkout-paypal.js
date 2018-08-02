window.paypalCheckoutReady = function () {
    if (document.getElementById('hdfPayPalAccountID')) {
        paypal.checkout.setup(document.getElementById('hdfPayPalAccountID').value, {
            environment: document.getElementById('hdfPayPalEnvironment').value,
            button: 'imgBtnConfirmPayPalPayment'
        });
    }
};