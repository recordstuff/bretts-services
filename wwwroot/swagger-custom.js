(() => {
    let observer;

    const createHeader = () => {
        const header = document.createElement("header");
        header.className = "brett-swagger-header";

        const portrait = document.createElement("img");
        portrait.className = "brett-swagger-header__portrait";
        portrait.src = "https://brettdrake.org/brett.jpg";
        portrait.alt = "Brett Drake";

        const portraitFrame = document.createElement("div");
        portraitFrame.className = "brett-swagger-header__portrait-frame";
        portraitFrame.append(portrait);

        const content = document.createElement("div");
        content.className = "brett-swagger-header__content";

        const title = document.createElement("h1");
        title.className = "brett-swagger-header__title";
        title.textContent = "Brett Drake's .NET API";

        const description = document.createElement("p");
        description.className = "brett-swagger-header__description";
        description.textContent = "This is Brett Drake's .NET API backend project using JWT Auth. The API hits a SQL Server instance running in another Docker container all running on LINUX's Docker Desktop. Although it would not ordinarily be enabled on a Production build, this Swagger page is enabled here to demonstrate the C# sample.";

        const githubLink = document.createElement("a");
        githubLink.className = "brett-swagger-header__github-link";
        githubLink.href = "https://github.com/recordstuff/bretts-services";
        githubLink.target = "_blank";
        githubLink.rel = "noopener noreferrer";

        const githubIcon = document.createElement("img");
        githubIcon.className = "brett-swagger-header__github-icon";
        githubIcon.src = "https://github.githubassets.com/favicons/favicon.svg";
        githubIcon.alt = "";
        githubIcon.setAttribute("aria-hidden", "true");

        const githubLabel = document.createElement("span");
        githubLabel.textContent = "See the GitHub Repo";

        githubLink.append(githubIcon, githubLabel);
        description.append(document.createTextNode(" "), githubLink);

        const siteLink = document.createElement("a");
        siteLink.className = "brett-swagger-header__site-link";
        siteLink.href = "https://brettdrake.org";
        siteLink.target = "_blank";
        siteLink.rel = "noopener noreferrer";
        siteLink.textContent = "brettdrake.org";

        description.append(document.createTextNode(" "), siteLink);
        content.append(title, description);
        header.append(portraitFrame, content);
        return header;
    };

    const addHeader = () => {
        if (document.querySelector(".brett-swagger-header")) {
            return true;
        }

        const topbar = document.querySelector("#swagger-ui .topbar");

        if (!topbar) {
            return false;
        }

        topbar.insertAdjacentElement("afterend", createHeader());
        return true;
    };

    const start = () => {
        if (addHeader()) {
            return;
        }

        const swaggerRoot = document.getElementById("swagger-ui");

        if (!swaggerRoot || observer) {
            return;
        }

        observer = new MutationObserver(() => {
            if (addHeader()) {
                observer.disconnect();
            }
        });

        observer.observe(swaggerRoot, { childList: true, subtree: true });
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", start, { once: true });
    } else {
        start();
    }

    window.addEventListener("load", start, { once: true });
})();
