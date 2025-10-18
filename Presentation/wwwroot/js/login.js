document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    console.log("Starting fetch request");

    const response = await fetch('http://localhost:5082/api/userprofiles/Login', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            Username: username,
            Password: password
        })
    });
    
    console.log("Fetch request completed");

    if (response.ok) {
        const data = await response.json();
        localStorage.setItem('currentUser', JSON.stringify(data));   // Add user to localStorage to be passed to the dashboard
        console.log(data);
        if (data.userProfileId < 3) {//if the id is less that x then its an admin
            window.location.href = '/Home/AdminDashboard';
        } else {
            window.location.href = '/Home/UserDashboard';
        }
    } else {
        document.getElementById('errorMessage').textContent = 'Login failed';
        console.error('Login failed:', response.statusText);
    }
});