
    function cofirm_mesf(form, msg) {
        $(function () {
            var Return = confirm(msg);
            if (Return) {
                form.submit();
            }
        });
    }