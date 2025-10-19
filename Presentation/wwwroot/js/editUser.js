document.addEventListener('DOMContentLoaded', () => {
    const profileJSON = localStorage.getItem('selectedUser');
    localStorage.removeItem('selectedUser'); // clear after retrieving

    if (!profileJSON) {
        window.location.href = '/Home/UserManagement';
        return;
    }

    const user = JSON.parse(profileJSON);
    const base64String = user.profilePicture;

    // Populate profile info
    document.getElementById('profileName').textContent = user.name;
    document.getElementById('profileEmail').textContent = user.email;
    document.getElementById('profilePhoneNumber').textContent = user.phoneNumber;
    document.getElementById('profileAddress').textContent = user.address;
    document.getElementById('profilePassword').textContent = user.password ? user.password : '********';
    document.getElementById('profilePic').src = `data:image/png;base64,${base64String}`;

    // Back button
    document.getElementById('backBtn').addEventListener('click', () => {
        window.location.href = '/Home/UserManagement';
    });

    // Reusable update function
    async function updateField(endpoint, newValue, fieldId, userProp, successMsg) {
        const errorBox = document.getElementById(fieldId);
        errorBox.textContent = '';
        console.log(fieldId, errorBox); // <-- debug


        if (!newValue) {
            errorBox.textContent = 'Please enter a new value.';
            return;
        }

        try {
            const response = await fetch(`http://localhost:5082/api/UserProfiles/${endpoint}/${user.userProfileId}/${encodeURIComponent(newValue)}`, {
                method: 'PUT'
            });

            if (response.ok) {
                user[userProp] = newValue;
                console.log(`profile${userProp.charAt(0).toUpperCase() + userProp.slice(1)}`); // <-- debug

                document.getElementById(`profile${userProp.charAt(0).toUpperCase() + userProp.slice(1)}`).textContent = newValue;
                errorBox.style.color = 'green';
                console.log('Before text content'); // <-- debug

                errorBox.textContent = successMsg;
                console.log('Successful'); // <-- debug

            } else {
                const errText = await response.text();
                errorBox.style.color = 'red';
                errorBox.textContent = errText;
            }
        } catch (err) {
            errorBox.textContent = 'Error: ' + err.message;
        }
    }

    // Update Buttons
    document.getElementById('updateNameBtn').addEventListener('click', () => {
        const newName = document.getElementById('newName').value;
        updateField('UpdateName', newName, 'nameError', 'name', 'Name updated successfully!');
    });

    document.getElementById('updateEmailBtn').addEventListener('click', () => {
        const newEmail = document.getElementById('newEmail').value;
        updateField('UpdateEmail', newEmail, 'emailError', 'email', 'Email updated successfully!');
    });

    document.getElementById('updatePhoneBtn').addEventListener('click', () => {
        const newPhone = document.getElementById('newPhone').value;
        updateField('UpdatePhoneNumber', newPhone, 'phoneError', 'phoneNumber', 'Phone number updated successfully!');
    });

    document.getElementById('updateAddressBtn').addEventListener('click', () => {
        const newAddress = document.getElementById('newAddress').value;
        updateField('UpdateAddress', newAddress, 'addressError', 'address', 'Address updated successfully!');
    });

    document.getElementById('updatePasswordBtn').addEventListener('click', () => {
        const newPassword = document.getElementById('newPassword').value;
        updateField('UpdatePassword', newPassword, 'passwordError', 'password', 'Password updated successfully!');
    });
});
