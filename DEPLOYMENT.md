# Deploy TradeFlow for free

Deploy the API to Render and the React dashboard to Vercel. PostgreSQL runs in Neon and Redis runs in Upstash.

## 1. Prepare cloud services

- Neon: use the pooled connection string for the `tradeflow` project.
- Upstash: create a free Redis database and copy its endpoint and password.

For StackExchange.Redis, set the Render Redis value in this form:

```text
your-upstash-host:6379,password=your-upstash-password,ssl=True,abortConnect=False
```

## 2. Deploy the API to Render

1. Push this repository to GitHub.
2. In Render, select **New > Blueprint** and choose this repository. Render reads `render.yaml`.
3. Enter the secret environment variables below. Do not commit them to GitHub.

| Key | Value |
| --- | --- |
| `ConnectionStrings__Postgres` | Neon pooled PostgreSQL connection string |
| `ConnectionStrings__Redis` | Upstash StackExchange.Redis connection string |
| `Jwt__SigningKey` | A new random secret at least 32 characters long |
| `Cors__AllowedOrigins` | Leave empty until Vercel gives you the frontend URL |

4. Deploy and copy the API URL, for example `https://tradeflow-api.onrender.com`.
5. Open `https://your-api-url/swagger` to confirm the API is running.

## 3. Deploy the dashboard to Vercel

1. In Vercel, import the same GitHub repository.
2. Set the **Root Directory** to `client`.
3. Set `VITE_API_URL` to your Render API URL, without a trailing slash.
4. Deploy and copy the Vercel production URL.

## 4. Allow the deployed dashboard

1. In Render, open the API service > Environment.
2. Set `Cors__AllowedOrigins` to your exact Vercel URL, for example:

```text
https://tradeflow.vercel.app
```

3. Save changes so Render redeploys the API.
4. Open Vercel URL, register a new user, and place a paper order.

## Notes

- Existing local PostgreSQL data stays local. The live app uses Neon data only.
- Free services can take time to wake after being idle.
- Never put database, Redis, or JWT secrets in `appsettings.json`, `.env.example`, or GitHub.
