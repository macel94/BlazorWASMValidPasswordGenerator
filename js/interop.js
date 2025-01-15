// Function to display a simple toast notification (you can customize this)
window.showToast = (message) => {
    // Create a div element for the toast
    const toast = document.createElement('div');
    toast.textContent = message;
    toast.style.position = 'fixed';
    toast.style.bottom = '20px';
    toast.style.right = '20px';
    toast.style.backgroundColor = '#333';
    toast.style.color = 'white';
    toast.style.padding = '10px';
    toast.style.borderRadius = '5px';
    toast.style.zIndex = '1000';

    // Add the toast to the body
    document.body.appendChild(toast);

    // Remove the toast after 3 seconds
    setTimeout(() => {
        document.body.removeChild(toast);
    }, 3000);
};