document.addEventListener('DOMContentLoaded', () => {
    const profileJSON = localStorage.getItem('currentUser');
    localStorage.clear(); // Clear the currentUser from localStorage after retrieving it
}