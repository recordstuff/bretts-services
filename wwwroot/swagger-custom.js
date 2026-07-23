(() => {
    const addHeader = () => {
        const swaggerContent = document.querySelector(".swagger-ui .information-container");

        if (!swaggerContent || document.querySelector(".brett-swagger-header")) {
            return false;
        }

        const header = document.createElement("header");
        header.className = "brett-swagger-header";

        const portrait = document.createElement("img");
        portrait.className = "brett-swagger-header__portrait";
        portrait.src = "/brett.jpg";
        portrait.alt = "Brett Drake";

        const content = document.createElement("div");
        content.className = "brett-swagger-header__content";

        const prompt = document.createElement("p");
        prompt.className = "brett-swagger-header__prompt";
        prompt.textContent = "$ curl brettdrake.org:8080/swagger";

        const title = document.createElement("h1");
        title.className = "brett-swagger-header__title";
        title.textContent = "Brett Drake's API";

        const description = document.createElement("p");
        description.className = "brett-swagger-header__description";
        description.textContent = "This is Brett Drake's working backend sample with real data.  The API hits a SQL Server instance running in a Docker Container on the host.  Since this is a sample for show and the data doesn't really matter, this Swagger page is enabled for the production build.";

        content.append(prompt, title, description);
        header.append(portrait, content);
        swaggerContent.parentNode.insertBefore(header, swaggerContent);
        return true;
    };

    if (addHeader()) {
        return;
    }

    const observer = new MutationObserver(() => {
        if (addHeader()) {
            observer.disconnect();
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });
})();
