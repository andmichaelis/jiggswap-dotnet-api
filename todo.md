✓ Require Address when accepting a trade
✓ Require Address when creating a trade

✓ Add "Forgot Password Token" to DB.Users.
✓ User Receives an Email containing $PASSWORD_TOKEN when uri:api/auth/ForgotPassword contains an email or username.
User is prompted to enter new password at uri:ui/reset-password.
✓ Set new password if valid $PASSWORD_TOKEN is provided at uri:api/auth/ResetPassword

User can Mark Trade as "Puzzle Shipped" after Trade is Active.
User can Mark Trade as "Puzzle Received" after BOTH "Puzzle Shipped".

Trade moves to Inactive when BOTH "Puzzle Received"

Add "Inactive Reason" to DB.Trades.
Inactive Reason is set to 'Declined By -User-' when Declined
Inactive Reason is set to 'Canceled By -User-' when Canceled
Inactive Reason is set to 'Completed' when Both Puzzles Received

User (Trade Recipient) receives a notification when Trade is Created.
User (Trade Initiator) receives a notification when Trade is Accepted.
User (Non-Current) receives a notification when Trade is Canceled or Declined.

✓ Privacy Policy.

Add Instructions for Client.
Replace Formik with [React-Hook-Form](https://react-hook-form.com/)
Replace React-Spring with [React-Simple-Animate](https://react-simple-animate.now.sh/)
