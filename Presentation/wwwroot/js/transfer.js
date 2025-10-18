document.addEventListener('DOMContentLoaded', async () => {
    const currentUserJSON = localStorage.getItem('currentUser');
    localStorage.clear(); // Clear the currentUser from localStorage after retrieving it

    if (!currentUserJSON) {
        window.location.href = '/Home/Index';
        return;
    }
    const currentUser = JSON.parse(currentUserJSON);

    const dropdown = document.getElementById('bankAccountDropdown');
    const messageBox = document.getElementById('transferMessage');

    // Fetch bank accounts helper function
    async function fetchAccounts() {
        const response = await fetch(`http://localhost:5082/api/BankAccounts/UserID/${currentUser.userProfileId}`);
        if (!response.ok) throw new Error("Failed to fetch bank accounts");
        return await response.json();
    }

    // Get all bank accounts for the user
    let accounts = await fetchAccounts();

    // Show bank accounts as a selectable drop down list
    accounts.forEach(acc => {
        const option = document.createElement('option');
        option.value = acc.bankAccountNumber;
        option.textContent = `${acc.bankAccountNumber} (${acc.bankName})`;
        dropdown.appendChild(option);
    });

    // Display first account by default
    if (accounts.length > 0) {
        dropdown.value = accounts[0].bankAccountNumber;
        showAccountInfo(accounts[0]);
    }

    // Update account info when new account selected
    dropdown.addEventListener('change', () => {
        const acc = accounts.find(a => a.bankAccountNumber == dropdown.value);
        if (acc) showAccountInfo(acc);
    });

    // Update the displayed account info
    function showAccountInfo(acc) {
        document.getElementById('accountNumber').textContent = acc.bankAccountNumber;
        document.getElementById('bankName').textContent = acc.bankName;
        document.getElementById('balance').textContent = acc.balance;
    }

    // Handle transfer
    document.getElementById('transferBtn').addEventListener('click', async () => {
        const fromAccount = dropdown.value;
        const toAccount = document.getElementById('toAccount').value.trim();
        const amount = parseFloat(document.getElementById('amount').value);
        const description = document.getElementById('description').value.trim();

        messageBox.style.color = 'red';

        // Input validation
        if (!toAccount || !amount || !description) {
            messageBox.textContent = "Please enter all required fields.";
            return;
        }

        if (isNaN(toAccount)) {
            messageBox.textContent = "Account number must be numeric.";
            return;
        }

        if (!/^\d{6}$/.test(toAccount)) {
            messageBox.textContent = "Account number must be exactly 6 digits (numbers only).";
            return;
        }

        if (!/^\d+(\.\d{1,2})?$/.test(amount)) {
            messageBox.textContent = "Amount must be a valid number with up to 2 decimal places.";
            return;
        }

        if (fromAccount === toAccount) {
            messageBox.textContent = "Cannot transfer between the same account.";
            return;
        }


        // Attempt to transfer
        try {
            const response = await fetch(`http://localhost:5082/api/Transactions/Transfer/${fromAccount}/${toAccount}/${amount}/${description}`, { method: 'PUT' });

            if (response.ok) {
                messageBox.style.color = 'green';
                messageBox.textContent = `Successfully transferred $${amount.toFixed(2)} to ${toAccount}.`;

                // Clear inputs
                document.getElementById('toAccount').value = '';
                document.getElementById('amount').value = '';
                document.getElementById('description').value = '';

                // Refresh user's account list to show updated balances
                accounts = await fetchAccounts();
                const updatedAccount = accounts.find(a => a.bankAccountNumber == fromAccount);
                if (updatedAccount) showAccountInfo(updatedAccount);
            } else {
                const error = await response.json();
                messageBox.textContent = error.message || "Transfer failed.";
            }
        } catch (err) {
            messageBox.textContent = "An error occurred during transfer.";
            console.error(err);
        }
    });

    // Back to Dashboard
    document.getElementById('backBtn').addEventListener('click', () => {
        localStorage.setItem('currentUser', JSON.stringify(currentUser));
        window.location.href = '/Home/UserDashboard';
    });
});