(function () {
    const counters = Array.from(document.querySelectorAll("[data-seat-counter]"));
    if (!counters.length || !window.signalR) {
        return;
    }

    const hubUrls = Array.from(document.querySelectorAll("[data-enrollment-hub-url], [data-enrollment-hub-url-api]"))
        .flatMap(element => [
            element.getAttribute("data-enrollment-hub-url"),
            element.getAttribute("data-enrollment-hub-url-api")
        ])
        .filter(url => url && url.trim().length > 0);

    const uniqueHubUrls = [...new Set(hubUrls.length ? hubUrls : ["/enrollmentHub"])];

    function updateCounters(sessionId, remainingSeats) {
        const normalizedSessionId = String(sessionId);
        const normalizedRemainingSeats = Number(remainingSeats);

        document
            .querySelectorAll(`[data-seat-counter][data-session-id="${normalizedSessionId}"]`)
            .forEach(counter => {
                const maxSeats = Number(counter.getAttribute("data-max-seats"));
                const remainingText = Number.isFinite(normalizedRemainingSeats)
                    ? normalizedRemainingSeats.toString()
                    : counter.getAttribute("data-remaining-seats") || "0";

                counter.setAttribute("data-remaining-seats", remainingText);
                counter.textContent = Number.isFinite(maxSeats)
                    ? `${remainingText} / ${maxSeats}`
                    : remainingText;

                counter.classList.toggle("bg-danger", Number(remainingText) <= 0);
                counter.classList.toggle("bg-success", Number(remainingText) > 0);
            });
    }

    uniqueHubUrls.forEach(hubUrl => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveSeatUpdate", updateCounters);

        connection.start().catch(error => {
            console.error("Could not connect to enrollment counter hub.", error);
        });
    });
})();
