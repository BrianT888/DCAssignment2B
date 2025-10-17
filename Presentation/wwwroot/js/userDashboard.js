document.addEventListener('DOMContentLoaded', async () => {
    const currentUserName = localStorage.getItem('currentUser');   // Get the current user's username
    console.log('currentUserName from localStorage:', currentUserName);

    if (!currentUserName) {
        // User not logged in, redirect to login page
        window.location.href = '/login';
        return;
    }

    try {
        const response = await fetch(`http://localhost:5082/api/userprofiles/Name/${currentUserName}`);
        console.log('Fetch response:', response);

        if (response.ok) {
            const userProfile = await response.json();
            console.log('Fetched userProfile:', userProfile);
            document.getElementById('userName').textContent = userProfile.name;
        }
        else {
            console.error('Failed to fetch user profile');
        }
    }
    catch (error) {
        console.error('Error fetching user profile: ', error);
    }
});