document.addEventListener('DOMContentLoaded', async () => {
    console.log('DOMContentLoaded event fired');
    const currentUserJSON = localStorage.getItem('currentUser');   // Get the current user's username
    localStorage.clear(); // Clear the currentUser from localStorage after retrieving it

    if (!currentUserJSON) {
        console.log("Redirect back to login")
        // User not logged in, redirect to login page
        window.location.href = '/Home/Index';
        return;
    }

    console.log('currentUserJSON parsing');
    const currentUser = JSON.parse(currentUserJSON);
    document.getElementById('userName').textContent = currentUser.name;

});