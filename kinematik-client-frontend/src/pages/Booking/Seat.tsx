const Seat = (props) => {
    return (
        <div
            className={classNames(classes['hall-layout-item'], getHallItemTypeClasses(layoutItem, selectedSeats))}
            style={
                {
                    '--hall-item-relative-width': widthRegistry[layoutItem.seatType],
                } as React.CSSProperties
            }
            onClick={() => {
                onSeatClick(layoutItem);
            }}
        >
            {getSeatTypeIcon(layoutItem.seatType)}
        </div>
    );
};

export default Seat;
