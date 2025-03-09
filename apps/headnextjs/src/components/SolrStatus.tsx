import type { GetServerSideComponentProps, GetStaticComponentProps } from "@sitecore-jss/sitecore-jss-nextjs";

const url = process.env.API_URL + "/solrcorestatus"

async function fetchApi() {
    const response = await fetch(url);
    const result = await response.json();
    return { cores: result };
}

export const getStaticProps: GetStaticComponentProps = async () => {
    return await fetchApi();
};

export const getServerSideProps: GetServerSideComponentProps = async () => {
    return await fetchApi();
};


type ComponentProps = {
    cores: Array<{ [key: string]: string }>;
};

export const Default = (props: ComponentProps): JSX.Element => {
    return (
        <div className="component">
            {props.cores.map(obj => (
                <div key={obj.name}>
                    <h2>SolR {obj.name} Status</h2>
                    <pre>{JSON.stringify(obj)}</pre>
                    {/* <dl>
                        {Object.keys(obj).map(key => (
                            <Fragment key={key}>
                                <dt>{key}</dt>
                                <dd>{obj[key]}</dd>
                            </Fragment>
                        ))}
                    </dl> */}
                </div>
            ))}
        </div>
    )
}
