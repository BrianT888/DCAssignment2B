document.addEventListener('DOMContentLoaded', () => {
    const profileJSON = localStorage.getItem('currentUser');
    localStorage.clear(); // Clear the currentUser from localStorage after retrieving it

    if (!profileJSON) {
        // No profile info, go back to login
        window.location.href = '/Home/Login';
        return;
    }

    const user = JSON.parse(profileJSON);

    const base64String = user.profilePicture;

    // Set profile info
    document.getElementById('profileName').textContent = user.name;
    document.getElementById('profileEmail').textContent = user.email;
    document.getElementById('profilePhone').textContent = user.phoneNumber;
    document.getElementById('profileAddress').textContent = user.address;
    document.getElementById('profilePic').src = `data:image/png;base64,${base64String}`;

    console.log(user.userProfileId);
    // Back button
    document.getElementById('backBtn').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(user));

        if (user.userProfileId < 3) {   // User is an admin if id < 3
            window.location.href = '/Home/AdminDashboard';
        }
        else {
            window.location.href = '/Home/UserDashboard';
        }
    });


    // Update Email
    document.getElementById('updateEmailBtn').addEventListener('click', async () => {
        const newEmail = document.getElementById('newEmail').value;
        const errorBox = document.getElementById('emailError');
        errorBox.textContent = ''; // clear previous error

        if (!newEmail) return alert('Enter a new email.');

        const response = await fetch(`http://localhost:5082/api/UserProfiles/UpdateEmail/${user.userProfileId}/${encodeURIComponent(newEmail)}`, { method: 'PUT'});

        if (response.ok) {
            user.email = newEmail;
            document.getElementById('profileEmail').textContent = newEmail;
            errorBox.textContent = 'Email updated successfully!';
        } else {
            const errText = await response.text();
            errorBox.textContent = errText;
        }
    });

    // Update Phone
    document.getElementById('updatePhoneBtn').addEventListener('click', async () => {
        const newPhone = document.getElementById('newPhone').value;
        const errorBox = document.getElementById('phoneError');
        errorBox.textContent = ''; // clear previous error

        if (!newPhone) return alert('Enter a new phone number.');

        const response = await fetch(`http://localhost:5082/api/UserProfiles/UpdatePhoneNumber/${user.userProfileId}/${encodeURIComponent(newPhone)}`, { method: 'PUT' });

        if (response.ok) {
            user.phoneNumber = newPhone;
            document.getElementById('profilePhone').textContent = newPhone;
            errorBox.textContent = 'Phone number updated successfully!';

        } else {
            const errText = await response.text();
            errorBox.textContent = errText;
        }
    });

    // Update Password
    document.getElementById('updatePasswordBtn').addEventListener('click', async () => {
        const newPassword = document.getElementById('newPassword').value;
        const errorBox = document.getElementById('passwordError');
        errorBox.textContent = ''; // clear previous error

        if (!newPassword) return alert('Enter a new password.');

        const response = await fetch(`http://localhost:5082/api/UserProfiles/UpdatePassword/${user.userProfileId}/${encodeURIComponent(newPassword)}`, { method: 'PUT' });

        if (response.ok) {
            errorBox.textContent = 'Phone number updated successfully!';
        } else {
            const errText = await response.text();
            errorBox.textContent = errText;
        }
    });
});