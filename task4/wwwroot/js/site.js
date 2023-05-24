
function deleteUsers(url, signinUrl) {
    let checkboxes = document.getElementsByName('userCheckbox');
    let checked = [...checkboxes].filter(x => x.checked === true).map(x => x.id);
    if (checked.length == 0) return;

    $.ajax({
        beforeSend: () => $('#loader').show(),
        complete: () => $('#loader').hide(),
        url: `${url}`,
        type: 'POST',
        cache: false,
        async: true,
        dataType: 'html',
        data: { "users": checked }
    })
        .done(result => {
            $('#usersTable').html(result)
        })
        .fail(result => {
            if (result.status === 401) {
                window.location.replace(signinUrl);
            }
        });

}