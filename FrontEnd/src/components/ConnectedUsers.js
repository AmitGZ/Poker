const ConnectedUsers = ({ users }) => (
    <div className='user-list'>
        {users.map((u, idx) => <h6 key={idx}>{u}</h6>)}
    </div>
);

export default ConnectedUsers;