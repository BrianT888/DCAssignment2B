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
        async function updateAccountDetails(accountNumber) {
            try {
                // Fetch transactions for this account
                const transactionsResponse = await fetch(`/api/Transactions/GetTransactionsForAccount/${accountNumber}`);
                if (!transactionsResponse.ok) throw new Error('Failed to fetch transactions');
                const transactions = await transactionsResponse.json();
                console.log('Fetched transactions:', transactions);

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
    } catch (err) {
        console.error('Error fetching bank accounts:', err);
    }

});