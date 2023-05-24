﻿

function headCheckboxChanged(changed) {
    let checkboxes = document.getElementsByName('userCheckbox');
    checkboxes.forEach(x => x.checked = changed.checked);
}

function deleteUsers(url, signinUrl) {
    let selected = getCheckedUsers();
    if (selected.length == 0) return;

    sendSelectedUsers(url, selected)
        .done(result => {
            refreshUsersTable(result);
        })
        .fail(result => {
            handleUnauthorized(result, signinUrl);
        });

}

function lockUsers(url, signinUrl) {
    let selected = getCheckedUsers();
    if (selected.length == 0) return;

    sendSelectedUsers(url, selected)
        .done(result => {
            refreshUsersTable(result);
        })
        .fail(result => {
            handleUnauthorized(result, signinUrl);
        });
}

function unlockUsers(url) {
    let selected = getCheckedUsers();
    if (selected.length == 0) return;

    sendSelectedUsers(url, selected)
        .done(result => {
            refreshUsersTable(result);
        });
}

function sendSelectedUsers(url, selected) {
    return $.ajax({
        beforeSend: () => $('#loader').show(),
        complete: () => $('#loader').hide(),
        url: `${url}`,
        type: 'POST',
        cache: false,
        async: true,
        dataType: 'html',
        data: { "users": selected }
    });
}

function refreshUsersTable(result) {
    $('#usersTable').html(result);
}

function handleUnauthorized(result, signinUrl) {
    if (result.status === 401) {
        window.location.replace(signinUrl);
    }
}

function getCheckedUsers() {
    let checkboxes = document.getElementsByName('userCheckbox');
    let checked = [...checkboxes].filter(x => x.checked === true).map(x => x.id);
    return checked;
}