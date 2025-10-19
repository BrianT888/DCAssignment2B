document.addEventListener("DOMContentLoaded", () => {
    console.log("Entered Transaction Management")
    const currentUserJSON = localStorage.getItem('currentUser');
    localStorage.clear(); // Clear currentUser from localStorage after retrieving it

    if (!currentUserJSON) {
        window.location.href = '/Home/Index'; // Redirect if not logged in
        return;
    }
    const adminProfile = JSON.parse(currentUserJSON);

    var transactions;

    async function loadTable() {
        try {
            // Fetch transactions
            const transactionsResponse = await fetch(`/api/Admin/GetAllTransactions`);
            if (!transactionsResponse.ok) throw new Error('Failed to fetch transactions');
            transactions = await transactionsResponse.json();

            // Update transaction table
            const tbody = document.getElementById('transactionTableBody');
            tbody.innerHTML = '';
            if (transactions.length === 0) {
                tbody.innerHTML = `<tr><td colspan="3" style="text-align:center;color:#999;">No transactions</td></tr>`;
            } else {
                transactions.sort((a, b) => new Date(b.date) - new Date(a.date)); // sort by newest first

                transactions.forEach(t => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                            <td>${new Date(t.date).toLocaleDateString('en-GB')}</td>
                            <td>${t.name}</td>
                            <td>${t.bankAccountNumber}</td>
                            <td>${t.description}</td>
                            <td>${t.amount}</td>
                        `;
                    tbody.appendChild(row);
                });
            }
        } catch (error) {
            console.error('Error updating account details:', error);
        }
    }

    loadTable();

    // -- Logic for filtering by date ---
    document.getElementById('filterBtn').addEventListener('click', () => {
        const fromDate = document.getElementById('fromDate').value;
        const toDate = document.getElementById('toDate').value;
        // Filter the list of transactions to be within the date range
        if (fromDate) transactions = transactions.filter(t => new Date(t.date) >= new Date(fromDate));
        if (toDate) transactions = transactions.filter(t => new Date(t.date) <= new Date(toDate));

        transactions.sort((a, b) => new Date(b.date) - new Date(a.date));
        updateTransactions(transactions);
    });

    // Display the new list of transactions
    function updateTransactions(transactions) {
        const tbody = document.getElementById('transactionTableBody');
        tbody.innerHTML = '';
        if (transactions.length === 0) {
            tbody.innerHTML = `<tr><td colspan="3" style="text-align:center;color:#999;">No transactions</td></tr>`;
            return;
        }

        transactions.forEach(t => {
            const row = document.createElement('tr');
            row.innerHTML = `
                    <td>${new Date(t.date).toLocaleDateString('en-GB')}</td>
                    <td>${t.name}</td>
                    <td>${t.bankAccountNumber}</td>
                    <td>${t.description}</td>
                    <td>${t.amount}</td>
                `;
            tbody.appendChild(row);
        });
    }

    //sort asc or desc for data
    document.getElementById('sortBtn').addEventListener('click', () => {
        const sortType = document.getElementById('sortType').value

        transactions.sort((t1, t2) => {
            const date1 = new Date(t1.date);
            const date2 = new Date(t2.date);

            if (sortType === 'ascending') {
                return date1 - date2;
            } else {
                return date2 - date1;
            }
        });

        updateTransactions(transactions);
    });

    // Reset the list to be unfiltered
    document.getElementById('resetFilterBtn').addEventListener('click', () => {
        document.getElementById('fromDate').value = '';
        document.getElementById('toDate').value = '';
        loadTable();
    });

    //Search by name
    document.getElementById('searchBtn').addEventListener('click', () => {
        const userSearch = document.getElementById('userSearch').value.toLowerCase();
        transactions = transactions.filter(t => t.name.toLowerCase().includes(userSearch));
        updateTransactions(transactions);
    });

    // Redirect
    document.getElementById('homeLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/adminDashboard';
    })

    // Redirect
    document.getElementById('userManagementLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/UserManagement';
    })

    // logout
    document.getElementById('logoutBtn').addEventListener('click', () => {
        window.location.href = '/Home/Index';
    })

    // Redirect to profile page when profile link clicked
    document.getElementById('profileLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(adminProfile));
        window.location.href = '/Home/Profile';
    })
});