document.addEventListener("DOMContentLoaded", async () => {
    // Get current admin from localStorage
    const currentUserJSON = localStorage.getItem('currentUser');
    localStorage.clear(); // Clear currentUser from localStorage after retrieving it

    if (!currentUserJSON) {
        window.location.href = '/Home/Index'; // Redirect if not logged in
        return;
    }
    const adminProfile = JSON.parse(currentUserJSON);

    const response = await fetch('http://localhost:5082/api/admin/CreateLog', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: adminProfile.userProfileId,
            msg: "Entered Home Page"
        })
    });

    if (!response.ok) throw new Error('Failed to Catch Log');
    // Display admin name
    document.getElementById('adminName').textContent = adminProfile.name;

    // Redirect
    document.getElementById('homeLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/adminDashboard';
    })

    // Redirect to profile page when profile link clicked
    document.getElementById('profileLink').addEventListener('click', async () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));

        const response = await fetch('http://localhost:5082/api/admin/CreateLog', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: adminProfile.userProfileId,
                msg: "Entered Profile Customisation"
            })
        });

        if (!response.ok) throw new Error('Failed to Catch Log');
        window.location.href = '/Home/Profile';
    })

    // Redirect to profile page when profile link clicked
    document.getElementById('userManagementLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/UserManagement';
    })

    // ----- Audit Log Logic -----
    try {
        const response = await fetch('http://localhost:5082/api/admin/GetAllLogs');
        if (!response.ok) throw new Error('Failed to fetch audit logs');

        const logs = await response.json();
        const auditTable = document.getElementById('auditTable');

        auditTable.innerHTML = ''; // Clear existing rows

        if (logs.length === 0) {
            auditTable.innerHTML = `<tr><td colspan="3" style="text-align:center; padding:20px; color:#999;">No logs yet</td></tr>`;
            return;
        }

        logs.forEach(log => {
            const row = document.createElement('tr');
            row.innerHTML = `
            <td>${log.logId}</td>
            <td>${new Date(log.date).toLocaleString('en-GB')}</td>
            <td>${log.logMsg}</td>
        `;
            auditTable.appendChild(row);
        });
    } catch (error) {
        console.error('Error loading audit logs:', error);
    }

    // Redirect
    document.getElementById('transactionManagementLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/TransactionManagement';
    })

    // logout
    document.getElementById('logoutBtn').addEventListener('click', async () => {
        const response = await fetch('http://localhost:5082/api/admin/CreateLog', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: adminProfile.userProfileId,
                msg: "User Logged out"
            })
        });

        if (!response.ok) throw new Error('Failed to Catch Log');
        window.location.href = '/Home/Index';
    })
});