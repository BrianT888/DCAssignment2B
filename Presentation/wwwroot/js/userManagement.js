document.addEventListener("DOMContentLoaded", () => {
    const tableBody = document.getElementById("usersTable");

    const searchIdBtn = document.getElementById("searchIdBtn");
    const searchIdInput = document.getElementById("searchIdInput");

    const searchNameBtn = document.getElementById("searchNameBtn");
    const searchNameInput = document.getElementById("searchNameInput");

    const searchEmailBtn = document.getElementById("searchEmailBtn");
    const searchEmailInput = document.getElementById("searchEmailInput");

    let allUsers = [];

    // --- Load all users on page load ---
    loadAllUsers();

    async function loadAllUsers() {
        try {
            const response = await fetch("http://localhost:5082/api/UserProfiles/GetAll");
            if (!response.ok) throw new Error("Failed to fetch users.");
            allUsers = await response.json();
            showUsers(allUsers);
        } catch (error) {
            console.error("Error loading users:", error);
            tableBody.innerHTML = `<tr><td colspan="5">Error loading users.</td></tr>`;
        }
    }

    // --- Show user table ---
    function showUsers(users) {
        // Filter out admin users (IDs 1 and 2)
        const filteredUsers = users.filter(u => u.userProfileId !== 1 && u.userProfileId !== 2);

        if (filteredUsers.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="5">No users found.</td></tr>`;
            return;
        }

        if (filteredUsers.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="5">No users found.</td></tr>`;
            return;
        }

        tableBody.innerHTML = "";
        filteredUsers.forEach(user => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${user.userProfileId}</td>
                <td>${user.name}</td>
                <td>${user.email}</td>
                <td>${user.isActive ? "Active" : "Inactive"}</td>
                <td>
                    <button class="btn-main edit-btn" data-id="${user.userProfileId}">Edit</button>
                    <button class="btn-main status-btn" data-id="${user.userProfileId}">
                        ${user.isActive ? "Deactivate" : "Activate"}
                    </button>
                </td>
            `;
            tableBody.appendChild(tr);
        });

        attachButtonHandlers();
    }

    // --- Search by ID ---
    searchIdBtn.addEventListener("click", () => {
        const term = searchIdInput.value.trim();
        if (!term) { showUsers(allUsers); return; }
        const filtered = allUsers.filter(u => u.userProfileId.toString() === term);
        showUsers(filtered);
    });

    // --- Search by Name ---
    searchNameBtn.addEventListener("click", () => {
        const term = searchNameInput.value.trim().toLowerCase();
        if (!term) { showUsers(allUsers); return; }
        const filtered = allUsers.filter(u => u.name && u.name.toLowerCase().includes(term));
        showUsers(filtered);
    });

    // --- Search by Email ---
    searchEmailBtn.addEventListener("click", () => {
        const term = searchEmailInput.value.trim().toLowerCase();
        if (!term) { showUsers(allUsers); return; }
        const filtered = allUsers.filter(u => u.email && u.email.toLowerCase().includes(term));
        showUsers(filtered);
    });

    // --- Button actions ---
    function attachButtonHandlers() {
        document.querySelectorAll(".edit-btn").forEach(btn => {
            btn.addEventListener("click", () => {
                const id = btn.dataset.id;
                const user = allUsers.find(u => u.userProfileId == id)
                if (!user) {
                    alert("User not found!");
                    return;
                }
                localStorage.setItem("selectedUser", JSON.stringify(user));
                window.location.href = `/Home/EditUser`;
            });
        });

        document.querySelectorAll(".status-btn").forEach(btn => {
            btn.addEventListener("click", async () => {
                const id = btn.dataset.id;
                const activate = btn.textContent.includes("Activate");
                const confirmMsg = activate ? "Activate this user?" : "Deactivate this user?";
                if (!confirm(confirmMsg)) return;

                try {
                    const response = await fetch(`http://localhost:5082/api/UserProfiles/ToggleStatus/${id}/${activate}`, { method: "PUT" });
                    if (response.ok) { alert("User status updated."); loadAllUsers(); }
                    else alert("Failed to update status.");
                } catch (err) {
                    console.error(err);
                    alert("Error updating status.");
                }
            });
        });
    }

    // --- CREATE NEW USER ---
    const createUserForm = document.getElementById("createUserForm");
    const createMsg = document.getElementById("createUserMsg");

    if (createUserForm) {
        createUserForm.addEventListener("submit", async (e) => {
            e.preventDefault();

            const name = document.getElementById("newUsername").value.trim();
            const email = document.getElementById("newEmail").value.trim();
            const phone = document.getElementById("newPhone").value.trim();
            const address = document.getElementById("newAddress").value.trim();
            const password = document.getElementById("newPassword").value.trim();

            // Input validation
            if (!name || !email || !phone || !address || !password) {
                showFeedback("All fields are required.", "red");
                return;
            }

            const newUser = {
                userProfileId: 0,
                name: name,
                email: email,
                phoneNumber: phone,
                address: address,
                password: password,
                profilePicture: null,
                isActive: true
            };

            try {
                const response = await fetch(`http://localhost:5082/api/Admin/CreateUser`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(newUser)
                });

                if (response.ok) {
                    showFeedback("User created successfully!", "green");
                    createUserForm.reset();
                    loadAllUsers(); // refresh table
                } else {
                    let errorMsg = "Unknown error";
                    try {
                        const errData = await response.json();
                        errorMsg = errData.message || JSON.stringify(errData);
                    } catch {
                        errorMsg = await response.text();
                    }
                    showFeedback(`Error: ${errorMsg}`, "red");
                }
            } catch (error) {
                showFeedback(`Error creating user: ${error.message}`, "red");
            }
        });
    }

    function showFeedback(message, color) {
        createMsg.textContent = message;
        createMsg.style.color = color;
    }

    // Redirect
    document.getElementById('homeLink').addEventListener('click', () => {
        window.location.href = '/Home/adminDashboard';
    })

    // Redirect
    document.getElementById('userManagementLink').addEventListener('click', () => {

        window.location.href = '/Home/UserManagement';
    })

    // logout
    document.getElementById('logoutBtn').addEventListener('click', () => {
        localStorage.clear();
        window.location.href = '/Home/Index';
    })

    // Redirect to profile page when profile link clicked
    document.getElementById('profileLink').addEventListener('click', () => {
        window.location.href = '/Home/Profile';
    })
});
