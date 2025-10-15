document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    // something like this idk bru

    //try {
    //    const response = await fetch('http://localhost:5082/api/auth/login', {
    //        method: 'POST',
    //        headers: { 'Content-Type': 'application/json' },
    //        body: JSON.stringify({ username, password })
    //    });

    //    const data = await response.json();

    //    if (data.isAdmin) {
    //        window.location.href = '/Home/AdminDashboard';
    //    } else {
    //        window.location.href = '/Home/UserDashboard';
    //    }
    //} catch (error) {
    //    document.getElementById('errorMessage').textContent = 'Login failed';
    //}

    // for now imma just hard code that if clicking the login button it goes to the user dashboard bru
    if (username && password) {
        window.location.href = '/Home/UserDashboard';
    } else {
        document.getElementById('errorMessage').textContent = 'Login failed';
    }
});