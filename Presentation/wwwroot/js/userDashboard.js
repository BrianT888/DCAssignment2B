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

        dropdown.addEventListener('change', async (e) => {
            const selectedAccountNumber = e.target.value; 
            if (!selectedAccountNumber) return;

            // Find selected bank account and display its information
            const account = accounts.find(a => a.bankAccountNumber == selectedAccountNumber);
            document.getElementById('accountNumber').textContent = account.bankAccountNumber;
            document.getElementById('bankName').textContent = account.bankName;
            document.getElementById('balance').textContent = account.balance.toFixed(2);

            // Fetch transactions for this account
            //try {
            //    const transResponse = await fetch(`http://localhost:5082/api/Transactions/Transactions/${selectedAccountNumber}`);
            //    const transactions = transResponse.ok ? await transResponse.json() : [];

            //    const tbody = document.getElementById('transactionTableBody');
            //    tbody.innerHTML = ''; // Clear previous rows

            //    if (transactions.length === 0) {
            //        tbody.innerHTML = `<tr><td colspan="3" style="text-align:center; padding:20px; color:#999;">No transactions yet</td></tr>`;
            //    } else {
            //        transactions.forEach(t => {
            //            const row = document.createElement('tr');
            //            row.innerHTML = `
            //                <td>${new Date(t.date).toLocaleString()}</td>
            //                <td>${t.description}</td>
            //                <td>${t.amount.toFixed(2)}</td>
            //            `;
            //            tbody.appendChild(row);
            //        });
            //    }
            //} catch (err) {
            //    console.error('Error fetching transactions:', err);
            //}
        });
    } catch (err) {
        console.error('Error fetching bank accounts:', err);
    }
});