# NexKart

A cross-platform e-commerce app built with .NET MAUI (C#, .NET 10) for Android, iOS, macOS and Windows.

- **Customers:** browse products, cart, wishlist, Stripe checkout, order history, profile.
- **Admins:** dashboard to manage products, orders and users.
- **Backend services:** Firestore (REST API through `HttpClient`), Firebase Authentication (email/password and Google sign-in), Stripe.

## Setup

### Stripe key

Create `NexKart/Resources/Raw/stripe_key.txt` containing your Stripe **test** secret key. The file is listed in `.gitignore` and must never be committed.

### Admin account

The app does not create an admin account on its own. To make someone an admin:

1. Register a normal account in the app.
2. In the Firebase console, open Firestore → `Users` → the document whose ID is that user's UID.
3. Set the field `Role` to `admin`.

The next time that user signs in, they land on the admin dashboard.
