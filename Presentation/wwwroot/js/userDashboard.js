document.addEventListener('DOMContentLoaded', async () => {
    const currentUserJSON = localStorage.getItem('currentUser');   // Get the current user's username
    localStorage.clear(); // Clear the currentUser from localStorage after retrieving it

    if (!currentUserJSON) {
        console.log("Redirect back to login")
        // User not logged in, redirect to login page
        window.location.href = '/Home/Index';
        return;
    }

    const currentUser = JSON.parse(currentUserJSON);

    // Set name
    document.getElementById('userName').textContent = currentUser.name;
    document.getElementById('accountName').textContent = currentUser.name;

    try {
        // Get all bank accounts for the user
        const accountsResponse = await fetch(`http://localhost:5082/api/BankAccounts/UserID/${currentUser.userProfileId}`);
        if (!accountsResponse.ok) throw new Error("Failed to fetch bank accounts");
        const accounts = await accountsResponse.json();

        // Show bank accounts as a selectable drop down list
        const dropdown = document.getElementById('bankAccountDropdown');
        accounts.forEach(acc => {
            const option = document.createElement('option');
            option.value = acc.bankAccountNumber;
            option.textContent = `${acc.bankAccountNumber} (${acc.bankName})`;
            dropdown.appendChild(option);
        })

        // Display first account by default
        if (accounts.length > 0) {
            dropdown.value = accounts[0].bankAccountNumber;
            updateAccountDetails(accounts[0].bankAccountNumber);
        }

        // Update account info when new account selected
        dropdown.addEventListener('change', async (e) => {
            const selectedAccountNumber = e.target.value; 
            if (!selectedAccountNumber) return;

            await updateAccountDetails(selectedAccountNumber);
        });

        // Displays latest information and transaction history of a bank account
        var transactions;
        async function updateAccountDetails(accountNumber) {
            try {
                // Fetch transactions for this account
                const transactionsResponse = await fetch(`/api/Transactions/GetTransactionsForAccount/${accountNumber}`);
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
                            <td>${t.description}</td>
                            <td>${t.amount}</td>
                        `;
                        tbody.appendChild(row);
                    });
                }

                // Get latest info for this account
                const account = accounts.find(a => a.bankAccountNumber == accountNumber);
                document.getElementById('accountNumber').textContent = account.bankAccountNumber;
                document.getElementById('bankName').textContent = account.bankName;
                document.getElementById('balance').textContent = account.balance;
            } catch (error) {
                console.error('Error updating account details:', error);
            }
        }


        // -- Logic for filtering by date ---
        document.getElementById('filterBtn').addEventListener('click', () => {
            const fromDate = document.getElementById('fromDate').value;
            const toDate = document.getElementById('toDate').value;
            let filtered = transactions;   // all transactions (for this account)

            // Filter the list of transactions to be within the date range
            if (fromDate) filtered = filtered.filter(t => new Date(t.date) >= new Date(fromDate));
            if (toDate) filtered = filtered.filter(t => new Date(t.date) <= new Date(toDate));

            updateTransactions(filtered);
        });

        // Reset the list to be unfiltered
        document.getElementById('resetFilterBtn').addEventListener('click', () => {
            document.getElementById('fromDate').value = '';
            document.getElementById('toDate').value = '';
            updateTransactions(transactions);
        });

        // Display the new (filtered) list of transactions
        function updateTransactions(transactions) {
            const tbody = document.getElementById('transactionTableBody');
            tbody.innerHTML = '';
            if (transactions.length === 0) {
                tbody.innerHTML = `<tr><td colspan="3" style="text-align:center;color:#999;">No transactions</td></tr>`;
                return;
            }

            transactions.sort((a, b) => new Date(b.date) - new Date(a.date)); // sort the list (newest first)
            transactions.forEach(t => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${new Date(t.date).toLocaleDateString('en-GB')}</td>
                    <td>${t.description}</td>
                    <td>${t.amount}</td>
                `;
                tbody.appendChild(row);
            });
        }
    } catch (err) {
        console.error('Error fetching bank accounts:', err);
    }


    // Redirect to profile page when profile link clicked
    document.getElementById('profileLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        window.location.href = '/Home/Profile';
    })

    // Redirect to transfer page when transfer link clicked
    document.getElementById('transferLink').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        window.location.href = '/Home/Transfer';
    });

});