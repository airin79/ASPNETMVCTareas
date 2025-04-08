
async function generatePdf() {
    const recipientEmail = document.getElementById("recipientEmail").value;  // Get recipient email
    const done = document.getElementById("done").value;
    const desde = document.getElementById("desde").value;
    const hasta = document.getElementById("hasta").value;

    // Create the URL parameters
    const params = new URLSearchParams({
        done: done,
        desde: desde,
        hasta: hasta,
        recipientEmail: recipientEmail  // Add recipient email as a parameter
    });

    /*
    const params = new URLSearchParams({
    done: document.getElementById("done").value,
    desde: document.getElementById("desde").value,
    hasta: document.getElementById("hasta").value
    });*/

    const url = "/Home/GeneratePdf?" + params.toString();
    console.log("Generated URL:", url);

    try {
            const response = await fetch(url, {
            method: 'GET',
            headers: {
            'Accept': 'application/pdf',   },
            });

    if (!response.ok) {
                throw new Error('Failed to generate PDF');
            }

    const pdfBlob = await response.blob();
    const link = document.createElement('a');
    link.href = URL.createObjectURL(pdfBlob);
    link.download = 'tareas.pdf';
    link.click();
        } catch (error) {
        console.error("Error generating PDF:", error);
        }
}