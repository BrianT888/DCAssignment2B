document.addEventListener("DOMContentLoaded", () => {
    // Get current admin from localStorage
    const currentUserJSON = localStorage.getItem('currentUser');
    localStorage.clear(); // Clear currentUser from localStorage after retrieving it

    if (!currentUserJSON) {
        window.location.href = '/Home/Index'; // Redirect if not logged in
        return;
    }
    const adminProfile = JSON.parse(currentUserJSON);

    // Display admin name
    document.getElementById('adminName').textContent = adminProfile.name;

    // Redirect to profile page when profile link clicked
    document.getElementById('profileLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/Profile';
    })

    // Redirect to profile page when profile link clicked
    document.getElementById('userManagementLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/UserManagement';
    })

    // ----- Audit Log Logic -----
    const auditLogs = [];

    const auditTable = document.getElementById('auditTable');

    auditLogs.forEach(log => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${log.date}</td>
            <td>${log.action}</td>
            <td>${log.user}</td>
        `;
        auditTable.appendChild(row);
    });

    // Redirect
    document.getElementById('transactionManagementLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/TransactionManagement';
    })

    // logout
    document.getElementById('logoutBtn').addEventListener('click', () => {
        //storage is already cleared at top so shouldn't need another line
        //localStorage.clear();
        window.location.href = '/Home/Index';
    })
});